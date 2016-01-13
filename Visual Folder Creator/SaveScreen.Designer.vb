<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SaveScreen
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.SaveScreenDialog = New System.Windows.Forms.SaveFileDialog()
        Me.SaveScreenButton = New System.Windows.Forms.Button()
        Me.QuitButton = New System.Windows.Forms.Button()
        Me.SaveScreen2Button = New System.Windows.Forms.Button()
        Me.FileAssociationButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'SaveScreenDialog
        '
        Me.SaveScreenDialog.DefaultExt = "VFCScreen"
        Me.SaveScreenDialog.Filter = "VFC Screen file|*.VFCScreen"
        '
        'SaveScreenButton
        '
        Me.SaveScreenButton.Dock = System.Windows.Forms.DockStyle.Top
        Me.SaveScreenButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SaveScreenButton.Location = New System.Drawing.Point(0, 0)
        Me.SaveScreenButton.Name = "SaveScreenButton"
        Me.SaveScreenButton.Size = New System.Drawing.Size(260, 65)
        Me.SaveScreenButton.TabIndex = 1
        Me.SaveScreenButton.Text = "Save As"
        Me.SaveScreenButton.UseVisualStyleBackColor = True
        '
        'QuitButton
        '
        Me.QuitButton.Dock = System.Windows.Forms.DockStyle.Top
        Me.QuitButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.QuitButton.Location = New System.Drawing.Point(0, 65)
        Me.QuitButton.Name = "QuitButton"
        Me.QuitButton.Size = New System.Drawing.Size(260, 65)
        Me.QuitButton.TabIndex = 4
        Me.QuitButton.Text = "Exit Without Saving"
        Me.QuitButton.UseVisualStyleBackColor = True
        '
        'SaveScreen2Button
        '
        Me.SaveScreen2Button.Dock = System.Windows.Forms.DockStyle.Top
        Me.SaveScreen2Button.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SaveScreen2Button.Location = New System.Drawing.Point(0, 130)
        Me.SaveScreen2Button.Name = "SaveScreen2Button"
        Me.SaveScreen2Button.Size = New System.Drawing.Size(260, 65)
        Me.SaveScreen2Button.TabIndex = 5
        Me.SaveScreen2Button.Text = "Save"
        Me.SaveScreen2Button.UseVisualStyleBackColor = True
        '
        'FileAssociationButton
        '
        Me.FileAssociationButton.Location = New System.Drawing.Point(201, 87)
        Me.FileAssociationButton.Name = "FileAssociationButton"
        Me.FileAssociationButton.Size = New System.Drawing.Size(126, 31)
        Me.FileAssociationButton.TabIndex = 0
        Me.FileAssociationButton.Text = "File Association"
        Me.FileAssociationButton.UseVisualStyleBackColor = True
        Me.FileAssociationButton.Visible = False
        '
        'SaveScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(260, 195)
        Me.Controls.Add(Me.SaveScreen2Button)
        Me.Controls.Add(Me.QuitButton)
        Me.Controls.Add(Me.SaveScreenButton)
        Me.Controls.Add(Me.FileAssociationButton)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SaveScreen"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Save"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SaveScreenDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents SaveScreenButton As System.Windows.Forms.Button
    Friend WithEvents QuitButton As System.Windows.Forms.Button
    Friend WithEvents SaveScreen2Button As System.Windows.Forms.Button
    Friend WithEvents FileAssociationButton As System.Windows.Forms.Button
End Class
