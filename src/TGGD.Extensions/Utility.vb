Public NotInheritable Class Utility
    Private Sub New()
    End Sub
    Public Shared Sub Repeat(iterations As Integer, activity As Action)
        For Each iteration In Enumerable.Range(1, iterations)
            activity.Invoke()
        Next
    End Sub
End Class
