Imports System.Runtime.CompilerServices
Imports Metaphor.Persistence

Public Module WorldExtensions
#Region "Abandoned House"
    <Extension>
    Private Sub CreateAbandonedHouse(world As IWorld, context As IInitializationContext)
        world.CreateLocation(
            LocationSubtypes.ABANDONED_HOUSE,
            "The Abandoned House",
            LocationInitializationExtensions.InitializeAbandonedHouse(context.ChosenName))
    End Sub
#End Region
#Region "Outside"
    <Extension>
    Friend Function CreateOutside(world As IWorld) As ILocation
        Return world.CreateLocation(LocationSubtypes.OUTSIDE, "Outside", AddressOf LocationInitializationExtensions.InitializeOutside)
    End Function
#End Region
#Region "Pay Toilet"
    <Extension>
    Friend Function CreatePayToilet(world As IWorld) As ILocation
        Return world.CreateLocation(LocationSubtypes.PAY_TOILET, "Pay Toilet", AddressOf LocationInitializationExtensions.InitializePayToilet)
    End Function
#End Region
#Region "Dark Alley"
    <Extension>
    Friend Function CreateDarkAlley(world As IWorld) As ILocation
        Return world.CreateLocation(LocationSubtypes.DARK_ALLEY, "Dark Alley", AddressOf LocationInitializationExtensions.InitializeDarkAlley)
    End Function
#End Region
    <Extension>
    Public Sub Initialize(world As IWorld, context As IInitializationContext)
        world.Clear()
        world.CreateAbandonedHouse(context)
        world.AddMessage("Welcome to Handies for Cash!")
        world.Avatar.Look()
    End Sub
End Module
