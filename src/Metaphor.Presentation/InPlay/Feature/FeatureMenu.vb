
Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class FeatureMenu
    Inherits MetaphorPickerMenu

    Private ReadOnly featureModel As IFeatureModel

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource, featureModel As IFeatureModel)
        MyBase.New(context, model, previous)
        Me.featureModel = featureModel
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return $"Do what with {featureModel.Name}?"
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Append(AddressOf ChooseNeverMind).
                Append(AddressOf ChooseItems).
                Concat(featureModel.Verbs.Select(AddressOf ChooseFeatureVerb))
        End Get
    End Property

    Private Function ChooseItems(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.Create(featureModel.Inventory.HasItems, "Items...", FeatureInventoryMenu.Launch(context, model, previous, featureModel))
    End Function

    Private Function ChooseFeatureVerb(verbModel As IVerbModel) As LaunchDelegate
        Return Function(c, m, p)
                   Return DialogChoice.Create(verbModel.IsEnabled, verbModel.Name, FeatureVerbActivity.Launch(c, m, p, featureModel, verbModel))
               End Function
    End Function

    Friend Shared Function Launch(c As IDisplayContext, m As IWorldModel, p As DialogSource, featureModel As IFeatureModel) As DialogSource
        Return Function()
                   If featureModel.Exists() Then
                       featureModel.Describe()
                       Return New FeatureMenu(c, m, p, featureModel)
                   End If
                   Return InPlay.Launch(c, m, p).Invoke()
               End Function
    End Function

    Private Function ChooseNeverMind(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Never Mind", InPlay.Launch(context, model, previous))
    End Function
End Class
