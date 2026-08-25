
Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class FeatureItemStackMenu
    Inherits MetaphorPickerMenu

    Private ReadOnly featureModel As IFeatureModel
    Private ReadOnly itemStackModel As IItemStackModel

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource, featureModel As IFeatureModel, itemStackModel As IItemStackModel)
        MyBase.New(context, model, previous)
        Me.featureModel = featureModel
        Me.itemStackModel = itemStackModel
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return "Items in stack:"
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseNeverMind).
                Append(AddressOf ChooseTake).
                Concat(itemStackModel.Items.Select(AddressOf ChooseItem))
        End Get
    End Property

    Private Function ChooseTake(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Take...", TakeFromFeatureItemStackPrompt.Launch(context, model, previous, featureModel, itemStackModel))
    End Function

    Private Function ChooseItem(itemModel As IItemModel) As LaunchDelegate
        Return Function(c, m, p)
                   Return DialogChoice.CreateEnabled(itemModel.Name, FeatureItemMenu.Launch(c, m, p, featureModel, itemModel))
               End Function
    End Function

    Friend Shared Function Launch(c As IDisplayContext, m As IWorldModel, p As DialogSource, featureModel As IFeatureModel, itemStackModel As IItemStackModel) As DialogSource
        Return Function()
                   Select Case itemStackModel.Count
                       Case 0
                           Return FeatureInventoryMenu.Launch(c, m, p, featureModel).Invoke()
                       Case 1
                           Return FeatureItemMenu.Launch(c, m, p, featureModel, itemStackModel.Top).Invoke()
                       Case Else
                           Return New FeatureItemStackMenu(c, m, p, featureModel, itemStackModel)
                   End Select
               End Function
    End Function

    Private Function ChooseNeverMind(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Never Mind", FeatureInventoryMenu.Launch(context, model, previous, featureModel))
    End Function
End Class
