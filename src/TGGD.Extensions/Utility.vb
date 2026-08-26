Public NotInheritable Class Utility
    Private Sub New() : End Sub
    Public Shared Sub Repeat(iterations As Integer, activity As Action)
        For Each iteration In Enumerable.Range(1, iterations)
            activity.Invoke()
        Next
    End Sub

    Public Shared Function Distance(fromXY As (X As Double, Y As Double), toXY As (X As Double, Y As Double)) As Double
        Return Math.Sqrt(Math.Pow(toXY.X - fromXY.X, 2.0) + Math.Pow(toXY.Y - fromXY.Y, 2.0))
    End Function

    Public Shared Function HeadingTo(fromXY As (X As Double, Y As Double), toXY As (X As Double, Y As Double)) As Double
        Return ToDegrees(Math.Atan2(toXY.Y - fromXY.Y, toXY.X - fromXY.X))
    End Function

    Public Shared Function GetNextXY(start As (X As Double, Y As Double), heading As Double, speed As Double) As (X As Double, Y As Double)
        Dim radians = ToRadians(heading)
        Dim deltaX As Double = Math.Cos(radians) * speed
        Dim deltaY As Double = Math.Sin(radians) * speed
        Return (start.X + deltaX, start.Y + deltaY)
    End Function

    Private Shared Function ToRadians(degrees As Double) As Double
        Return degrees * Math.PI / 180.0
    End Function
    Private Shared Function ToDegrees(radians As Double) As Double
        Dim degrees = radians * 180.0 / Math.PI
        Return If(degrees < 0, degrees + 360.0, degrees)
    End Function
End Class
