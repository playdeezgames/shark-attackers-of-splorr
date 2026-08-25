Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class ItemVerbActivity
    Inherits MetaphorPickerMenu

    Private ReadOnly verbModel As IVerbModel

    Public Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource, itemModel As IItemModel, verbModel As IVerbModel)
        MyBase.New(context, model, previous)
        Me.verbModel = verbModel
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return String.Empty
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseOk)
        End Get
    End Property

    Private Function ChooseOk(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Ok", InPlay.Launch(context, model, previous))
    End Function

    Friend Shared Function Launch(c As IDisplayContext, m As IWorldModel, p As DialogSource, itemModel As IItemModel, verbModel As IVerbModel) As DialogSource
        Return Function()
                   verbModel.Perform()
                   Return New ItemVerbActivity(c, m, p, itemModel, verbModel)
               End Function
    End Function

End Class
