Imports Metaphor.Presentation
Imports Metaphor.Processing
Imports TGGD.Persistence
Imports TGGD.Platform

Public Class MetaphorDisplay
    Inherits Display

    Private ReadOnly quittable As Boolean
    Private ReadOnly persister As IPersister

    Private Sub New(quittable As Boolean, persister As IPersister)
        Me.quittable = quittable
        Me.persister = persister
    End Sub

    Public Overrides Async Function Start() As Task
        UpdateDialog(Title.Launch(Me, Await WorldModel.Create(quittable, persister), Function() Nothing).Invoke())
    End Function

    Public Shared Async Function Create(quittable As Boolean, persister As IPersister) As Task(Of IDisplay)
        Dim result = New MetaphorDisplay(quittable, persister)
        Await result.Start()
        Return result
    End Function

End Class
