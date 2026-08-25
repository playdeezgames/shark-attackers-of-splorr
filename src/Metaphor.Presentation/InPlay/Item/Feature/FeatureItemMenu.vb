
Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class FeatureItemMenu
    Inherits MetaphorPickerMenu

    Private ReadOnly featureModel As IFeatureModel
    Private ReadOnly itemModel As IItemModel

    Public Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource, featureModel As IFeatureModel, itemModel As IItemModel)
        MyBase.New(context, model, previous)
        Me.featureModel = featureModel
        Me.itemModel = itemModel
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return $"Do what with {itemModel.Name}?"
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseNeverMind).
                Append(AddressOf ChooseTake)
        End Get
    End Property

    Private Function ChooseTake(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled(
            "Take",
            FeatureTakeItemActivity.Launch(
                context,
                model,
                previous,
                featureModel, itemModel))
    End Function

    Friend Shared Function Launch(c As IDisplayContext, m As IWorldModel, p As DialogSource, featureModel As IFeatureModel, itemModel As IItemModel) As DialogSource
        Return Function() New FeatureItemMenu(c, m, p, featureModel, itemModel)
    End Function

    Private Function ChooseNeverMind(
                                    context As IDisplayContext,
                                    model As IWorldModel,
                                    previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled(
            "Never Mind",
            FeatureInventoryMenu.Launch(
                context,
                model,
                previous,
                featureModel))
    End Function
End Class
