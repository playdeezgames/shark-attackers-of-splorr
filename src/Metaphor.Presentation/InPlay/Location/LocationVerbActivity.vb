Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class LocationVerbActivity
    Inherits MetaphorDialog

    Private ReadOnly verbModel As IVerbModel

    Public Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource, verbModel As IVerbModel)
        MyBase.New(context, model, previous)
        Me.verbModel = verbModel
    End Sub

    Friend Shared Function Launch(c As IDisplayContext, m As IWorldModel, p As DialogSource, verbModel As IVerbModel) As DialogSource
        Return Function() New LocationVerbActivity(c, m, p, verbModel)
    End Function

    Public Overrides Function Run() As IDialogPrompt
        verbModel.Perform()
        Return InPlay.Launch(Context, Model, Previous).Invoke().Run()
    End Function
End Class
