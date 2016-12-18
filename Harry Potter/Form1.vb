﻿Imports System.IO
Imports System.Net

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        setUpPlayer()
        getLocation(True)
    End Sub

    Private Sub PauseToolStripMenuItem_Click(sender As Object, e As EventArgs)
        AxWindowsMediaPlayer1.Ctlcontrols.pause()
    End Sub

    '################# OPEN FILE #################
    Private Sub OpenFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenFileToolStripMenuItem.Click
        Dim ofd As New OpenFileDialog
        If (ofd.ShowDialog() = DialogResult.OK) Then
            My.Settings.fileLoc = ofd.FileName
            My.Settings.Save()
            AxWindowsMediaPlayer1.URL = My.Settings.fileLoc
        Else
            MsgBox("Error, File not found or supported", MsgBoxStyle.Exclamation, "AudioBook Player")
        End If
        ofd.Dispose()
    End Sub

    Private Sub LastPositionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LastPositionToolStripMenuItem.Click
        AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = My.Settings.lastAuTime - 5
    End Sub
    Dim Last As String

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ManSaved.Text = My.Settings.lastManTime.ToString.Split(".")(0) + "s Manual Saved"
        ManSaving.Text = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition.ToString.Split(".")(0) + "s Save"
        AutoSave.Text = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition.ToString.Split(".")(0) + "s AUTO SAVING"
        AutoSaved.Text = My.Settings.lastAuTime.ToString.Split(".")(0) + "s Auto Saved"

        SpeedToolStripMenuItem.Text = "Speed x" + AxWindowsMediaPlayer1.settings.rate.ToString("0.00")

        If Last <> AxWindowsMediaPlayer1.Ctlcontrols.currentPosition.ToString Then
            Last = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition.ToString
            PlayToolStripMenuItem.Text = "Pause"
            PlayToolStripMenuItem.Image = My.Resources._1454111120_pause_circle_fill
        Else
            PlayToolStripMenuItem.Text = "Play"
            PlayToolStripMenuItem.Image = My.Resources._1454111105_play_circle_fill
        End If

        My.Settings.lastAuTime = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition
        My.Settings.Save()
    End Sub

    Private Sub AutoSavedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutoSaved.Click
        AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = My.Settings.lastAuTime
    End Sub

    Private Sub ManualSavedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ManSaved.Click
        AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = My.Settings.lastManTime
    End Sub

    Private Sub SaveLocationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ManSaving.Click
        My.Settings.lastManTime = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition
        My.Settings.Save()
        okDone = My.Settings.lastManTime
        upload()
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        upload()
        SleepToolStripMenuItem.BackColor = Color.LimeGreen
        System.Diagnostics.Process.Start("shutdown", "-s -t 00")
    End Sub

    Private Sub SleepToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SleepToolStripMenuItem.Click
        SleepToolStripMenuItem.BackColor = Color.LimeGreen
    End Sub

    Private Sub OnlineSavedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OnlineSavedToolStripMenuItem.Click
        getLocation(False)
    End Sub

    Private Sub MinToolStripMenuItem2_Click(sender As Object, e As EventArgs)

    End Sub



    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        My.Settings.Save()
        Me.Close()
    End Sub

    Dim okDone As String




    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        upload()
    End Sub


    '+++++++++++++++++++++++++++++++++++++++++++++++ MY FUNCTIONS '+++++++++++++++++++++++++++++++++++++++++++++++

    '################### OPENS LAST AUDIO FILE AND LAST SAVED LOCATION ###################
    Private Sub setUpPlayer()
        AxWindowsMediaPlayer1.settings.volume = 100
        If (My.Settings.fileLoc.ToString <> "") Then
            AxWindowsMediaPlayer1.URL = My.Settings.fileLoc.ToString
            If (My.Settings.lastAuTime.ToString <> "") Then
                AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = My.Settings.lastAuTime
                If (AxWindowsMediaPlayer1.Ctlcontrols.currentPosition > 5) Then
                    AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition - 5
                    PlayToolStripMenuItem.Text = "Pause"
                    PlayToolStripMenuItem.Image = My.Resources._1454111120_pause_circle_fill
                End If
                AxWindowsMediaPlayer1.Ctlcontrols.play()
            End If
        Else
            AxWindowsMediaPlayer1.Ctlcontrols.stop()
        End If
    End Sub

    '############### SHUTDOWN PC ###############
    Private Sub shutDown(tome As Integer)
        Timer2.Interval = tome
        Timer2.Start()
    End Sub

    '############ SHUT DOWN APP ################
    Private Sub shutDownApp(tome As Integer)
        Timer3.Interval = tome
        Timer3.Start()
    End Sub




    '+++++++++++++++++++++++++++++++++++++++++++++++ ONLINE BOOKMARK '+++++++++++++++++++++++++++++++++++++++++++++++

    '############### UPLOAD TO SERVER ##############
    Private Sub upload()
        Try
            If okDone <> "" Then
                Dim request As WebRequest = WebRequest.Create("" & "<html><body>" & okDone & "</body></html>")
                request.GetResponse()
            End If

        Catch ex As Exception

        End Try
        getLocation(True)
    End Sub

    '############### DOWNLOAD FROM SERVER ##############
    Private Sub getLocation(locate As Boolean)
        Try
            Dim address As String = ""
            Dim client As WebClient = New WebClient()
            Dim reader As StreamReader = New StreamReader(client.OpenRead(address))
            Dim loc As String = reader.ReadToEnd

            Dim spiltOne As String() = New String() {"</body>"}
            Dim spiltTwo As String() = New String() {"<body>"}
            Dim shit As Integer = loc.Split(spiltOne, False)(0).Split(spiltTwo, False)(1)

            OnlineSavedToolStripMenuItem.Text = Convert.ToInt32(shit) & " Online Saved"
            If (locate) Then
            Else
                AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = Convert.ToInt32(shit)
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub





    '+++++++++++++++++++++++++++++++++++++++++++++++ CONTROL BUTTONS '+++++++++++++++++++++++++++++++++++++++++++++++

    '################# STOP BUTTON #################
    Private Sub StopToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopToolStripMenuItem.Click
        AxWindowsMediaPlayer1.Ctlcontrols.stop()
    End Sub

    '################# PLAY BUTTON #################
    Private Sub PlayToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayToolStripMenuItem.Click
        If PlayToolStripMenuItem.Text = "Play" Then
            AxWindowsMediaPlayer1.Ctlcontrols.play()
            PlayToolStripMenuItem.Text = "Pause"
            PlayToolStripMenuItem.Image = My.Resources._1454111120_pause_circle_fill
        Else
            AxWindowsMediaPlayer1.Ctlcontrols.pause()
            PlayToolStripMenuItem.Text = "Play"
            PlayToolStripMenuItem.Image = My.Resources._1454111105_play_circle_fill
        End If
    End Sub

    '################# REWIND #################
    Private Sub SToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles SToolStripMenuItem.Click, SToolStripMenuItem1.Click, SToolStripMenuItem2.Click, SToolStripMenuItem6.Click, SToolStripMenuItem7.Click, MinToolStripMenuItem.Click
        Dim STS As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition - Int(STS.Text.Replace("s", "").ToString)
    End Sub

    '################# FORWARD #################
    Private Sub SToolStripMenuItem3_Click_1(sender As Object, e As EventArgs) Handles SToolStripMenuItem3.Click, SToolStripMenuItem4.Click, SToolStripMenuItem5.Click, SToolStripMenuItem8.Click, SToolStripMenuItem9.Click, MinToolStripMenuItem1.Click
        Dim STS As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition + Int(STS.Text.Replace("s", "").ToString)
    End Sub

    '################ SHUTDOWN PC ##############
    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click, ToolStripMenuItem3.Click, ToolStripMenuItem4.Click, HourToolStripMenuItem.Click, HoursToolStripMenuItem1.Click, HoursToolStripMenuItem.Click
        Dim STS As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        shutDown(Int(STS.Text.Replace("min", "").ToString) * 60000)
    End Sub

    '################ SHUTDOWN APP ##############
    Private Sub MinToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles MinToolStripMenuItem4.Click, MinToolStripMenuItem3.Click, MinToolStripMenuItem2.Click, HourToolStripMenuItem1.Click, HourToolStripMenuItem2.Click, HoursToolStripMenuItem2.Click
        Dim STS As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        shutDownApp(Int(STS.Text.Replace("min", "").ToString) * 60000)
    End Sub

    '############## STOP SHUTDOWNS ##############
    Private Sub StopShutDownToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopShutDownToolStripMenuItem.Click
        Process.Start(Application.ExecutablePath)
        Me.Close()
    End Sub
    '################## SPEED ###################
    Private Sub NormalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NormalToolStripMenuItem.Click, X05ToolStripMenuItem.Click, X12ToolStripMenuItem.Click, X13ToolStripMenuItem.Click, X14ToolStripMenuItem.Click, X15ToolStripMenuItem.Click, X20ToolStripMenuItem.Click, X11ToolStripMenuItem.Click
        Dim STS As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        If AxWindowsMediaPlayer1.settings.isAvailable("Rate") Then
            If (STS.Text <> "Normal") Then
                AxWindowsMediaPlayer1.settings.rate = Convert.ToDouble(STS.Text.Replace("x", "").ToString)
            Else
                AxWindowsMediaPlayer1.settings.rate = 1
            End If
            AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition - 15
        End If
    End Sub
End Class


'<?php
'$msg = $_GET['w'];
'$logfile= 'data.txt';
'$fp = fopen($logfile, "a");
'fwrite($fp, $msg);
'fclose($fp);
'?>