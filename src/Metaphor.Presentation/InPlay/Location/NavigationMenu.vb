Imports Metaphor.Processing
Imports TGGD.Presentation

Friend Class NavigationMenu
    Inherits MetaphorPickerMenu

    Private Sub New(context As IDisplayContext, model As IWorldModel, previous As DialogSource)
        MyBase.New(context, model, previous)
    End Sub

    Public Overrides ReadOnly Property PromptText As String
        Get
            Return "Now What?"
        End Get
    End Property

    Protected Overrides ReadOnly Property Launchers As IEnumerable(Of LaunchDelegate)
        Get
            Return Enumerable.Empty(Of LaunchDelegate).
                Concat(Model.Location.AvailableVerbs.Select(AddressOf ChooseLocationVerb)).
                Concat(Model.Avatar.AvailableVerbs.Select(AddressOf ChooseAvatarVerb)).
                Append(AddressOf ChooseStatus).
                Append(AddressOf ChooseGround).
                Append(AddressOf ChooseInventory).
                Concat(Model.Location.Characters.All.Select(AddressOf ChooseCharacter)).
                Concat(Model.Location.Features.AllVisible.Select(AddressOf ChooseFeature)).
                Append(AddressOf ChooseLook).
                Append(AddressOf InPlay.ChooseWatchAd).
                Append(AddressOf InPlay.ChooseGameMenu)
        End Get
    End Property

    Private Function ChooseCharacter(characterModel As ICharacterModel) As LaunchDelegate
        Return Function(c, m, p)
                   Return DialogChoice.Create(
                        True,
                        $"{characterModel.Name}...",
                        CharacterMenu.Launch(c, m, p, characterModel))
               End Function
    End Function

    Private Function ChooseFeature(featureModel As IFeatureModel) As LaunchDelegate
        Return Function(c, m, p)
                   Return DialogChoice.Create(
                        featureModel.Enabled,
                        $"{featureModel.Name}...",
                        FeatureMenu.Launch(c, m, p, featureModel))
               End Function
    End Function

    Private Function ChooseStatus(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Status...", StatusActivity.LaunchStatusActivity(context, model, previous))
    End Function

    Private Shared Function ChooseAvatarVerb(verbModel As IVerbModel) As LaunchDelegate
        Return Function(c, m, p) DialogChoice.Create(verbModel.IsEnabled, verbModel.Name, AvatarVerbActivity.Launch(c, m, p, verbModel))
    End Function

    Private Function ChooseLocationVerb(verbModel As IVerbModel) As LaunchDelegate
        Return Function(c, m, p) DialogChoice.Create(verbModel.IsEnabled, verbModel.Name, LocationVerbActivity.Launch(c, m, p, verbModel))
    End Function

    Private Function ChooseInventory(
                                    context As IDisplayContext,
                                    model As IWorldModel,
                                    previous As DialogSource) As IDialogChoice
        Return DialogChoice.Create(model.Avatar.Inventory.HasItems, "Inventory...", InventoryMenu.Launch(context, model, previous))
    End Function

    Private Function ChooseLook(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.CreateEnabled("Look", LookActivity.Launch(context, model, previous))
    End Function

    Private Function ChooseGround(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As IDialogChoice
        Return DialogChoice.Create(model.Location.Ground.HasItems, "Ground...", GroundMenu.Launch(context, model, previous))
    End Function

    Friend Shared Function Launch(context As IDisplayContext, model As IWorldModel, previous As DialogSource) As DialogSource
        Return Function() New NavigationMenu(context, model, previous)
    End Function
End Class
