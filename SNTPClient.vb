'*
'* A C# SNTP Client
'* 
'* Copyright (C)2001-2003 Valer BOCAN <vbocan@dataman.ro>
'* All Rights Reserved
'* 
'* VB.NET port by Ray Frankulin <random0000@cox.net>
'*
'* You may download the latest version from http://www.dataman.ro/sntp
'* If you find this class useful and would like to support my existence, please have a
'* look at my Amazon wish list at
'* http://www.amazon.com/exec/obidos/wishlist/ref=pd_wt_3/103-6370142-9973408
'* or make a donation to my Delta Forth .NET project, at
'* http://shareit1.element5.com/product.html?productid=159082&languageid=1&stylefrom=159082&backlink=http%3A%2F%2Fwww.dataman.ro&currencies=USD
'* 
'* Last modified: September 20, 2003
'*  
'* Permission is hereby granted, free of charge, to any person obtaining a
'* copy of this software and associated documentation files (the
'* "Software"), to deal in the Software without restriction, including
'* without limitation the rights to use, copy, modify, merge, publish,
'* distribute, and/or sell copies of the Software, and to permit persons
'* to whom the Software is furnished to do so, provided that the above
'* copyright notice(s) and this permission notice appear in all copies of
'* the Software and that both the above copyright notice(s) and this
'* permission notice appear in supporting documentation.
'*
'* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
'* OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
'* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT
'* OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
'* HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL
'* INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING
'* FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT,
'* NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION
'* WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
'*
'* Disclaimer
'* ----------
'* Although reasonable care has been taken to ensure the correctness of this
'* implementation, this code should never be used in any application without
'* proper verification and testing. I disclaim all liability and responsibility
'* to any person or entity with respect to any loss or damage caused, or alleged
'* to be caused, directly or indirectly, by the use of this SNTPClient class.
'*
'* Comments, bugs and suggestions are welcome.
'*
'* Update history:
'* September 20, 2003
'* - Renamed the class from NTPClient to SNTPClient.
'* - Fixed the RoundTripDelay and LocalClockOffset properties.
'*   Thanks go to DNH <dnharris@csrlink.net>.
'* - Fixed the PollInterval property.
'*   Thanks go to Jim Hollenhorst <hollenho@attbi.com>.
'* - Changed the ReceptionTimestamp variable to DestinationTimestamp to follow the standard
'*   more closely.
'* - Precision property is now shown is seconds rather than milliseconds in the
'*   ToString method.
'* 
'* May 28, 2002
'* - Fixed a bug in the Precision property and the SetTime function.
'*   Thanks go to Jim Hollenhorst <hollenho@attbi.com>.
'* 
'* March 14, 2001
'* - First public release.
'*/

Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.InteropServices
Imports System.Text

Namespace InternetTime
    'Leap indicator field values
    Public Enum _LeapIndicator
        NoWarning       '0 - No warning
        LastMinute61    '1 - Last minute has 61 seconds
        LastMinute59    '2 - Last minute has 59 seconds
        Alarm           '3 - Alarm condition (clock not synchronized)
    End Enum

    'Mode field values
    Public Enum _Mode
        SymmetricActive     '1 - Symmetric active
        SymmetricPassive    '2 - Symmetric pasive
        Client              '3 - Client
        Server              '4 - Server
        Broadcast           '5 - Broadcast
        Unknown             '0, 6, 7 - Reserved
    End Enum

    'Stratum field values
    Public Enum _Stratum
        Unspecified         '0 - unspecified or unavailable
        PrimaryReference    '1 - primary reference (e.g. radio-clock)
        SecondaryReference  '2-15 - secondary reference (via NTP or SNTP)
        Reserved            '16-255 - reserved
    End Enum

    '/// <summary>
    '/// SNTPClient is a VB.NET# class designed to connect to time servers on the Internet and
    '/// fetch the current date and time. Optionally, it may update the time of the local system.
    '/// The implementation of the protocol is based on the RFC 2030.
    '/// 
    '/// Public class members:
    '///
    '/// LeapIndicator - Warns of an impending leap second to be inserted/deleted in the last
    '/// minute of the current day. (See the _LeapIndicator enum)
    '/// 
    '/// VersionNumber - Version number of the protocol (3 or 4).
    '/// 
    '/// Mode - Returns mode. (See the _Mode enum)
    '/// 
    '/// Stratum - Stratum of the clock. (See the _Stratum enum)
    '/// 
    '/// PollInterval - Maximum interval between successive messages
    '/// 
    '/// Precision - Precision of the clock
    '/// 
    '/// RootDelay - Round trip time to the primary reference source.
    '/// 
    '/// RootDispersion - Nominal error relative to the primary reference source.
    '/// 
    '/// ReferenceID - Reference identifier (either a 4 character string or an IP address).
    '/// 
    '/// ReferenceTimestamp - The time at which the clock was last set or corrected.
    '/// 
    '/// OriginateTimestamp - The time at which the request departed the client for the server.
    '/// 
    '/// ReceiveTimestamp - The time at which the request arrived at the server.
    '/// 
    '/// Transmit Timestamp - The time at which the reply departed the server for client.
    '/// 
    '/// RoundTripDelay - The time between the departure of request and arrival of reply.
    '/// 
    '/// LocalClockOffset - The offset of the local clock relative to the primary reference
    '/// source.
    '/// 
    '/// Initialize - Sets up data structure and prepares for connection.
    '/// 
    '/// Connect - Connects to the time server and populates the data structure.
    '///	It can also update the system time.
    '/// 
    '/// IsResponseValid - Returns true if received data is valid and if comes from
    '/// a NTP-compliant time server.
    '/// 
    '/// ToString - Returns a string representation of the object.
    '/// 
    '/// -----------------------------------------------------------------------------
    '/// Structure of the standard NTP header (as described in RFC 2030)
    '///                       1                   2                   3
    '///   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '///  |LI | VN  |Mode |    Stratum    |     Poll      |   Precision   |
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '///  |                          Root Delay                           |
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '///  |                       Root Dispersion                         |
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '///  |                     Reference Identifier                      |
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '///  |                                                               |
    '///  |                   Reference Timestamp (64)                    |
    '///  |                                                               |
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '///  |                                                               |
    '///  |                   Originate Timestamp (64)                    |
    '///  |                                                               |
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '///  |                                                               |
    '///  |                    Receive Timestamp (64)                     |
    '///  |                                                               |
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '///  |                                                               |
    '///  |                    Transmit Timestamp (64)                    |
    '///  |                                                               |
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '///  |                 Key Identifier (optional) (32)                |
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '///  |                                                               |
    '///  |                                                               |
    '///  |                 Message Digest (optional) (128)               |
    '///  |                                                               |
    '///  |                                                               |
    '///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '/// 
    '/// -----------------------------------------------------------------------------
    '/// 
    '/// SNTP Timestamp Format (as described in RFC 2030)
    '///                         1                   2                   3
    '///     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    '/// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '/// |                           Seconds                             |
    '/// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '/// |                  Seconds Fraction (0-padded)                  |
    '/// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    '/// 
    '/// </summary>

    Public Class SNTPClient

        '// NTP Data Structure Length
        Private Const SNTPDataLength As Byte = 47

        '// NTP Data Structure (as described in RFC 2030)
        Dim SNTPData(SNTPDataLength) As Byte

        '// Offset constants for timestamps in the data structure
        Private Const offReferenceID As Byte = 12
        Private Const offReferenceTimestamp As Byte = 16
        Private Const offOriginateTimestamp As Byte = 24
        Private Const offReceiveTimestamp As Byte = 32
        Private Const offTransmitTimestamp As Byte = 40

        'Leap Indicator
        Public ReadOnly Property LeapIndicator() As _LeapIndicator
            Get
                'Isolate the two most significant bits
                Dim bVal As Byte = (SNTPData(0) >> 6)
                Select Case bVal
                    Case 0 : Return _LeapIndicator.NoWarning
                    Case 1 : Return _LeapIndicator.LastMinute61
                    Case 2 : Return _LeapIndicator.LastMinute59
                    Case 3 : Return _LeapIndicator.Alarm
                    Case Else : Return _LeapIndicator.Alarm
                End Select
            End Get
        End Property

        ' Version Number
        Public ReadOnly Property VersionNumber() As Byte
            Get
                'Isolate bits 3 - 5
                Dim bVal As Byte = (SNTPData(0) And &H38) >> 3
                Return bVal
            End Get
        End Property

        Public ReadOnly Property Mode()
            Get
                'Isolate bits 0 - 3
                Dim bVal As Byte = (SNTPData(0) And &H7)
                Select Case bVal
                    Case 0, 6, 7
                        Return _Mode.Unknown
                    Case 1
                        Return _Mode.SymmetricActive
                    Case 2
                        Return _Mode.SymmetricPassive
                    Case 3
                        Return _Mode.Client
                    Case 4
                        Return _Mode.Server
                    Case 5
                        Return _Mode.Broadcast
                End Select
            End Get
        End Property

        'Stratum
        Public ReadOnly Property Stratum() As _Stratum
            Get
                Dim bVal As Byte = SNTPData(1)
                If (bVal = 0) Then
                    Return _Stratum.Unspecified
                ElseIf (bVal = 1) Then
                    Return _Stratum.PrimaryReference
                ElseIf (bVal <= 15) Then
                    Return _Stratum.SecondaryReference
                Else
                    Return _Stratum.Reserved
                End If
            End Get
        End Property

        'Poll Interval
        Public ReadOnly Property PollInterval() As Int32
            Get
                '// Thanks to Jim Hollenhorst <hollenho@attbi.com>
                Return Math.Pow(2, SNTPData(2))
                'Return Math.Round(Math.Pow(2, SNTPData(2)))
            End Get
        End Property

        'Precision (in milliseconds)
        Public ReadOnly Property Precision() As Double
            Get
                '// Thanks to Jim Hollenhorst <hollenho@attbi.com>
                Return Math.Pow(2, SNTPData(3))
                'Return (1000 * Math.Pow(2, SNTPData(3) - 256))
            End Get
        End Property

        'Root Delay (in milliseconds)
        Public ReadOnly Property RootDelay() As Double
            Get
                Dim temp As Int64 = 0
                temp = 256 * (256 * (256 * SNTPData(4) + SNTPData(5)) + SNTPData(6)) + SNTPData(7)
                Return 1000 * ((temp) / &H10000)
            End Get
        End Property

        'Root Dispersion (in milliseconds)
        Public ReadOnly Property RootDispersion() As Double
            Get
                Dim temp As Int64 = 0
                temp = 256 * (256 * (256 * SNTPData(8) + SNTPData(9)) + SNTPData(10)) + SNTPData(11)
                Return 1000 * ((temp) / &H10000)
            End Get
        End Property

        'Reference Identifier
        Public ReadOnly Property ReferenceID() As String
            Get
                Dim val As String = ""
                Select Case Stratum
                    Case _Stratum.PrimaryReference Or Stratum.Unspecified
                        If SNTPData(offReferenceID + 0) <> 0 Then val += Chr(SNTPData(offReferenceID + 0))
                        If SNTPData(offReferenceID + 1) <> 0 Then val += Chr(SNTPData(offReferenceID + 1))
                        If SNTPData(offReferenceID + 2) <> 0 Then val += Chr(SNTPData(offReferenceID + 2))
                        If SNTPData(offReferenceID + 3) <> 0 Then val += Chr(SNTPData(offReferenceID + 3))
                    Case _Stratum.SecondaryReference
                        Select Case VersionNumber
                            Case 3 '// Version 3, Reference ID is an IPv4 address
                                Dim Address As String = SNTPData(offReferenceID + 0).ToString() + "." + SNTPData(offReferenceID + 1).ToString() + "." + SNTPData(offReferenceID + 2).ToString() + "." + SNTPData(offReferenceID + 3).ToString()
                                Try
                                    Dim Host As IPHostEntry = Dns.GetHostByAddress(Address)
                                    val = Host.HostName + " (" + Address + ")"
                                Catch e As Exception
                                    val = "N/A"
                                End Try
                            Case 4 '// Version 4, Reference ID is the timestamp of last update
                                Dim time As DateTime = ComputeDate(GetMilliSeconds(offReferenceID))
                                '// Take care of the time zone
                                Dim offspan As TimeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)
                                val = (time.Add(offspan)).ToString()
                            Case Else
                                val = "N/A"
                        End Select
                End Select
                Return val
            End Get
        End Property

        '// Reference Timestamp
        Public ReadOnly Property ReferenceTimestamp() As DateTime
            Get
                Dim time As DateTime = ComputeDate(GetMilliSeconds(offReferenceTimestamp))
                '// Take care of the time zone
                Dim offspan As TimeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)
                Return time.Add(offspan)
            End Get
        End Property

        '// Originate Timestamp
        Public ReadOnly Property OriginateTimestamp() As DateTime
            Get
                Return ComputeDate(GetMilliSeconds(offOriginateTimestamp))
            End Get
        End Property

        '// Receive Timestamp
        Public ReadOnly Property ReceiveTimestamp() As DateTime
            Get
                Dim time As DateTime = ComputeDate(GetMilliSeconds(offReceiveTimestamp))
                'Take care of the time zone
                Dim offspan As TimeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)
                Return time.Add(offspan)
            End Get
        End Property

        '// Transmit Timestamp
        Public Property TransmitTimestamp() As DateTime
            Get
                Dim time As DateTime = ComputeDate(GetMilliSeconds(offTransmitTimestamp))
                'Take care of the time zone
                Dim offspan As TimeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)
                Return time.Add(offspan)
            End Get
            Set(ByVal Value As DateTime)
                SetDate(offTransmitTimestamp, Value)
            End Set
        End Property

        '// Destination Timestamp
        Public DestinationTimestamp As DateTime

        '// Round trip delay (in milliseconds)
        Public ReadOnly Property RoundTripDelay() As Int64
            Get
                '// Thanks to DNH <dnharris@csrlink.net>
                Dim span As TimeSpan = DestinationTimestamp.Subtract(OriginateTimestamp).Subtract(ReceiveTimestamp.Subtract(TransmitTimestamp))
                Return span.TotalMilliseconds
            End Get
        End Property

        '// Local clock offset (in milliseconds)
        Public ReadOnly Property LocalClockOffset() As Int64
            Get
                '// Thanks to DNH <dnharris@csrlink.net>
                Dim span As TimeSpan = ReceiveTimestamp.Subtract(OriginateTimestamp).Add((TransmitTimestamp.Subtract(DestinationTimestamp)))
                Return span.TotalMilliseconds / 2
            End Get
        End Property

        '// Compute date, given the number of milliseconds since January 1, 1900
        Private Function ComputeDate(ByVal milliseconds As Decimal) As DateTime
            Dim span As TimeSpan = TimeSpan.FromMilliseconds(milliseconds)
            Dim time As DateTime = New DateTime(1900, 1, 1)
            time = time.Add(span)
            Return time
        End Function

        '// Compute the number of milliseconds, given the offset of a 8-byte array
        Private Function GetMilliSeconds(ByVal offset As Byte) As Decimal
            Dim intPart As Decimal = 0, fractPart As Decimal = 0
            Dim i As Int32
            For i = 0 To 3
                intPart = Int(256 * intPart + SNTPData(offset + i))
            Next
            For i = 4 To 7
                fractPart = Int(256 * fractPart + SNTPData(offset + i))
            Next
            Dim milliseconds As Decimal = Int(intPart * 1000 + (fractPart * 1000) / &H100000000L)
            Return milliseconds
        End Function

        '// Compute the 8-byte array, given the date
        Private Sub SetDate(ByVal offset As Byte, ByVal dateval As DateTime)
            Dim intPart As Decimal = 0, fractPart As Decimal = 0
            Dim StartOfCentury As DateTime = New DateTime(1900, 1, 1, 0, 0, 0)
            Dim milliseconds As Decimal = Int(dateval.Subtract(StartOfCentury).TotalMilliseconds)
            intPart = Int(milliseconds / 1000)
            fractPart = Int(((milliseconds Mod 1000) * &H100000000L) / 1000)
            Dim temp As Decimal = intPart
            Dim i As Decimal
            For i = 3 To 0 Step -1
                SNTPData(offset + i) = Int(temp Mod 256)
                temp = Int(temp / 256)
            Next
            temp = Int(fractPart)
            For i = 7 To 4 Step -1
                SNTPData(offset + i) = Int(temp Mod 256)
                temp = Int(temp / 256)
            Next
        End Sub

        '// Initialize the NTPClient data
        Private Sub Initialize()
            'Set version number to 4 and Mode to 3 (client)
            SNTPData(0) = &H1B
            'Initialize all other fields with 0
            Dim i As Int32
            For i = 1 To 47
                SNTPData(i) = 0
            Next
            'Initialize the transmit timestamp
            TransmitTimestamp = DateTime.Now
        End Sub

        Public Sub New(ByVal host As String)
            TimeServer = host
        End Sub

        '// Connect to the time server and update system time
        Public Function Connect(ByVal UpdateSystemTime As Boolean) As Boolean
            Try
                'Resolve server address
                Dim hostadd As IPHostEntry = Dns.Resolve(TimeServer)
                Dim EPhost As IPEndPoint = New IPEndPoint(hostadd.AddressList(0), 123)

                'Connect the time server
                Dim TimeSocket As UdpClient = New UdpClient
                TimeSocket.Connect(EPhost)

                'Initialize data structure
                Initialize()
                TimeSocket.Send(SNTPData, SNTPData.Length)
                SNTPData = TimeSocket.Receive(EPhost)
                If IsResponseValid() = False Then
                    Throw New Exception("Invalid response from " + TimeServer)
                End If
                DestinationTimestamp = DateTime.Now
            Catch e As SocketException
                Throw New Exception(e.Message)
            End Try

            '// Update system time
            If (UpdateSystemTime) Then
                SetTime()
            End If
        End Function

        '// Check if the response from server is valid
        Public Function IsResponseValid() As Boolean
            If (SNTPData.Length < SNTPDataLength Or Mode <> _Mode.Server) Then
                Return False
            Else
                Return True
            End If
        End Function

        '// Converts the object to string
        Public Overrides Function ToString() As String
            Dim str As String
            Dim sb As New StringBuilder("")

            sb.Append("Leap Indicator: ")
            Select Case LeapIndicator
                Case _LeapIndicator.NoWarning
                    sb.Append("No warning")
                Case _LeapIndicator.LastMinute61
                    sb.Append("Last minute has 61 seconds")
                Case _LeapIndicator.LastMinute59
                    sb.Append("Last minute has 59 seconds")
                Case _LeapIndicator.Alarm
                    sb.Append("Alarm Condition (clock not synchronized)")
            End Select
            sb.Append(vbCrLf & "Version number: " + VersionNumber.ToString())
            sb.Append(vbCrLf & "Mode: ")
            Select Case Mode
                Case _Mode.Unknown
                    sb.Append("Unknown")
                Case _Mode.SymmetricActive
                    sb.Append("Symmetric Active")
                Case _Mode.SymmetricPassive
                    sb.Append("Symmetric Pasive")
                Case _Mode.Client
                    sb.Append("Client")
                Case _Mode.Server
                    sb.Append("Server")
                Case _Mode.Broadcast
                    sb.Append("Broadcast")
            End Select
            sb.Append(vbCrLf & "Stratum: ")
            Select Case Stratum
                Case _Stratum.Unspecified
                Case _Stratum.Reserved
                    sb.Append("Unspecified")
                Case _Stratum.PrimaryReference
                    sb.Append("Primary Reference")
                Case _Stratum.SecondaryReference
                    sb.Append("Secondary Reference")
            End Select
            sb.Append(vbCrLf & "Local time: " + TransmitTimestamp.ToString())
            sb.Append(vbCrLf & "Precision: " + Precision.ToString() + " ms")
            sb.Append(vbCrLf & "Poll Interval: " + PollInterval.ToString() + " s")
            sb.Append(vbCrLf & "Reference ID: " + ReferenceID.ToString())
            sb.Append(vbCrLf & "Root Delay: " + RootDelay.ToString() + " ms")
            sb.Append(vbCrLf & "Root Dispersion: " + RootDispersion.ToString() + " ms")
            sb.Append(vbCrLf & "Round Trip Delay: " + RoundTripDelay.ToString() + " ms")
            sb.Append(vbCrLf & "Local Clock Offset: " + LocalClockOffset.ToString() + " ms")
            sb.Append(vbCrLf)

            Return sb.ToString
        End Function

        '// SYSTEMTIME structure used by SetSystemTime
        <StructLayout(LayoutKind.Sequential)> Private Structure SYSTEMTIME
            Public year As Int16
            Public month As Int16
            Public dayOfWeek As Int16
            Public day As Int16
            Public hour As Int16
            Public minute As Int16
            Public second As Int16
            Public milliseconds As Int16
        End Structure

        <DllImport("KERNEL32.DLL", EntryPoint:="SetLocalTime", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=False, CallingConvention:=CallingConvention.StdCall)> Private Shared Function SetLocalTime(ByRef time As SYSTEMTIME) As Int32
        End Function

        '// Set system time according to transmit timestamp
        Private Sub SetTime()
            Dim x As Boolean
            Dim st As SYSTEMTIME
            Dim trts As DateTime = DateTime.Now.AddMilliseconds(LocalClockOffset)
            st.year = trts.Year
            st.month = trts.Month
            st.dayOfWeek = trts.DayOfWeek
            st.day = trts.Day
            st.hour = trts.Hour
            st.minute = trts.Minute
            st.second = trts.Second
            st.milliseconds = trts.Millisecond
            SetLocalTime(st)
        End Sub

        '// The URL of the time server we're connecting to
        Private TimeServer As String
    End Class
End Namespace