
Imports System.IO
Imports System.Net




Public Class frMain


    Dim IniFile As String
    Dim LastDate As Date


    Private Sub SendFTP()
        Try

            Dim request As FtpWebRequest = DirectCast(WebRequest.Create(txt_Server.Text & "/" & txt_File.Text), System.Net.FtpWebRequest) 'On renseigne la futur destination du fichier à envoyer
            request.Credentials = New NetworkCredential(txt_UserName.Text, txt_Password.Text) 'On rentre les identifiant et mot de passe
            request.Method = System.Net.WebRequestMethods.Ftp.UploadFile 'On indique qu'on veut upload un fichier
            Dim files() As Byte = File.ReadAllBytes(lblFile.Text) 'On indique le chemin du fichier à upload
            Dim strz As Stream = request.GetRequestStream() 'On créer un stream qui va nous permettre d'envoyer le fichier
            strz.Write(files, 0, files.Length) 'On envoie le fichier
            strz.Close() 'On ferme la connection
            strz.Dispose() 'On supprime la connection

            ToolStripStatusLabel1.Text = "Last Send: " & Date.Now.ToString
            ToolStripStatusLabel1.BackColor = Color.Transparent

        Catch ex As Exception
            ToolStripStatusLabel1.Text = "Error : " & ex.Message
            ToolStripStatusLabel1.BackColor = Color.Red
        End Try
    End Sub

    'Choose a file and save name in ini file
    Private Sub bt_File_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bt_File.Click
        With OpenFileDialog1
            .FileName = ""
            .Filter = "All Files (*.*) | *.*"
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                lblFile.Text = .FileName
                Cls_Ini.INIWrite(IniFile, "Local", "File", .FileName)
            End If
        End With
    End Sub

    Private Sub chk_Send_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk_Send.CheckedChanged
        ToolStripProgressBar1.Visible = chk_Send.Checked
        Timer1.Enabled = chk_Send.Checked
        If Timer1.Enabled Then
            checkAndSend()
        End If
    End Sub

    'load last option in ini file
    Private Sub frMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
       
        IniFile = Application.StartupPath & "\LastOption.ini"
        If Cls_Ini.INISectionExist(IniFile, "FTP") Then
            txt_Server.Text = Cls_Ini.INIRead(IniFile, "FTP", "Server")
            txt_File.Text = Cls_Ini.INIRead(IniFile, "FTP", "File")
            txt_UserName.Text = Cls_Ini.INIRead(IniFile, "FTP", "UserName")
            txt_Password.Text = Cls_Ini.INIRead(IniFile, "FTP", "PassWord")
        End If
        If Cls_Ini.INISectionExist(IniFile, "Local") Then
            lblFile.Text = Cls_Ini.INIRead(IniFile, "Local", "File")
            txt_Second.Text = Cls_Ini.INIRead(IniFile, "Local", "Second")
        End If

    End Sub

    'Save FTP option in ini file
    Private Sub bt_save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bt_save.Click

        Cls_Ini.INIWrite(IniFile, "FTP", "Server", txt_Server.Text)
        Cls_Ini.INIWrite(IniFile, "FTP", "File", txt_File.Text)
        Cls_Ini.INIWrite(IniFile, "FTP", "UserName", txt_UserName.Text)
        Cls_Ini.INIWrite(IniFile, "FTP", "PassWord", txt_Password.Text)

    End Sub



    Private Sub checkAndSend()
        If LastDate < File.GetLastWriteTime(lblFile.Text) Then
            LastDate = File.GetLastWriteTime(lblFile.Text)
            SendFTP()
        End If
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        If ToolStripProgressBar1.Value = ToolStripProgressBar1.Maximum Then
            ToolStripProgressBar1.Value = 0
        End If

        ToolStripProgressBar1.Value += 1

        If ToolStripProgressBar1.Value = ToolStripProgressBar1.Maximum Then
            checkAndSend()
        End If
    End Sub

    Private Sub txt_second_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_Second.TextChanged
        If Not IsNumeric(txt_Second.Text) Then
            txt_Second.BackColor = Color.Pink
        Else
            txt_Second.BackColor = Color.GreenYellow
            ToolStripProgressBar1.Maximum = txt_Second.Text
            Cls_Ini.INIWrite(IniFile, "Local", "Second", txt_Second.Text)
        End If

    End Sub
End Class
