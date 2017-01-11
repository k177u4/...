Public Class Form1
    Dim WithEvents gService As BR.AN.PviServices.Service
    Dim WithEvents gCPU As BR.AN.PviServices.Cpu
    Dim WithEvents gTask As BR.AN.PviServices.Task
    Dim WithEvents gVariable As BR.AN.PviServices.Variable

    Sub LogActivity(ByVal pProcess As String)
        Console.WriteLine(pProcess)
    End Sub

    Private Sub gService_Connected(ByVal sender As Object, ByVal e As BR.AN.PviServices.PviEventArgs) Handles gService.Connected
        If gService.IsConnected = True Then
            LogActivity("Service connected. Establishing connection to controller...")

            ' Establish connection to the controller
            gCPU = New BR.AN.PviServices.Cpu(gService, "cpu")
            With gCPU.Connection
                .DeviceType = BR.AN.PviServices.DeviceType.TcpIp
                .TcpIp.DestinationStation = 2
                .TcpIp.DestinationPort = 11159
                .TcpIp.DestinationIpAddress = "172.30.41.44"
            End With
            gCPU.Connect()
        End If
    End Sub

    Private Sub gService_Error(ByVal sender As Object, ByVal e As BR.AN.PviServices.PviEventArgs) Handles gService.Error
        LogActivity("Service error: " & e.ErrorCode & "-" & e.ErrorText)
    End Sub


    Private Sub gCPU_Connected(ByVal sender As Object, ByVal e As BR.AN.PviServices.PviEventArgs) Handles gCPU.Connected
        If gCPU.HasError = False Then
            If gCPU.IsConnected = True Then
                LogActivity("Connected to controller. Retrieving values...")

                gTask = New BR.AN.PviServices.Task(gCPU, "timebase")
                gTask.Connect()
            End If
        End If
    End Sub

    Private Sub gCPU_Error(ByVal sender As Object, ByVal e As BR.AN.PviServices.PviEventArgs) Handles gCPU.Error
        LogActivity("Controller error: " & e.ErrorCode & "-" & e.ErrorText)
    End Sub

    Private Sub gTask_Connected(ByVal sender As Object, ByVal e As BR.AN.PviServices.PviEventArgs) Handles gTask.Connected
        gVariable = New BR.AN.PviServices.Variable(gTask, "timebase")
        gVariable.UserData = 1
        gVariable.Active = False
        gVariable.RefreshTime = 1000
        gVariable.Connect()
    End Sub

    Private Sub gTask_Error(ByVal sender As Object, ByVal e As BR.AN.PviServices.PviEventArgs) Handles gTask.Error
        LogActivity("Controller error: " & e.ErrorCode & "-" & e.ErrorText)
    End Sub

    Private Sub gVariable_Connected(ByVal sender As Object, ByVal e As BR.AN.PviServices.PviEventArgs) Handles gVariable.Connected
        Dim myVar As BR.AN.PviServices.Variable
        myVar = sender
        myVar.Active = True
    End Sub

    Private Sub gVariable_ValueChanged(ByVal sender As Object, ByVal e As BR.AN.PviServices.PviEventArgs) Handles gVariable.ValueChanged
        Dim myVar As BR.AN.PviServices.Variable
        myVar = sender

        Dim newList As List(Of String) = TextBox1.Lines.ToList

        While (newList.Count > 15)
            newList.RemoveAt(0)
        End While

        newList.Add(e.Action.ToString() + ": " + myVar.FullName + ":" + myVar.Value.ToString())
        TextBox1.Lines = newList.ToArray
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        gService = New BR.AN.PviServices.Service("service")
        gService.Connect("192.1.168.31", 55354)        ' Connect to extruder control with IP 172.31.57.124
        'This is a test change
    End Sub
End Class
