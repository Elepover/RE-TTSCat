
Public Class Window_Loading
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim DesktopWorkingArea = SystemParameters.WorkArea
        Me.Left = DesktopWorkingArea.Right - Me.Width - 10
        Me.Top = DesktopWorkingArea.Bottom - Me.Height - 10
    End Sub
End Class
