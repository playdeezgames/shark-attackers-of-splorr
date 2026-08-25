Imports TGGD.Processing

Public MustInherit Class BaseModelDialog(Of TContext As IDisplayContext, TModel As IModel)
    Inherits BaseDialog(Of TContext)

    Protected ReadOnly Model As TModel

    Protected Sub New(context As TContext, model As TModel)
        MyBase.New(context)
        Me.Model = model
    End Sub
End Class
