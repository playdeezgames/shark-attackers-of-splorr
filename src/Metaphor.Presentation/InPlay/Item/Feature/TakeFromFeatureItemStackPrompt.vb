Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class TakeFromFeatureItemStackPrompt
    Inherits MetaphorDialog

    Private ReadOnly itemStackModel As IItemStackModel
    Private ReadOnly featureModel As IFeatureModel

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource, featureModel As IFeatureModel, itemStackModel As IItemStackModel)
        MyBase.New(context, model, previous)
        Me.itemStackModel = itemStackModel
        Me.featureModel = featureModel
    End Sub

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource, featureModel As IFeatureModel, itemStackModel As IItemStackModel) As DialogSource
        Return Function() New TakeFromFeatureItemStackPrompt(context, model, previous, featureModel, itemStackModel)
    End Function

    Public Overrides Function Run() As IDialogPrompt
        Return DialogPrompt.CreateIntegerPrompt($"How many {itemStackModel.Top.Name} are you taking? (Available: {itemStackModel.Count})", AddressOf ChooseQuantity)
    End Function

    Private Function ChooseQuantity(value As Integer) As IDialog
        itemStackModel.Take(value)
        Return FeatureItemStackMenu.Launch(Context, Model, Previous, featureModel, itemStackModel).Invoke()
    End Function
End Class
