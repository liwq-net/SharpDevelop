﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// Pop-up menu for context actions; used for Find derived classes (F6) and commands that open popups.
	/// </summary>
	public class ContextActionsPopup : Popup
	{
		public ContextActionsPopup()
		{
			// Close on lost focus
			this.StaysOpen = false;
			this.AllowsTransparency = true;
			this.ActionsControl = new ContextActionsHeaderedControl();
			// Close when any action excecuted
			this.ActionsControl.ActionExecuted += delegate { this.IsOpen = false; };
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Key == Key.Escape)
				this.IsOpen = false;
		}
		
		public ContextActionsHeaderedControl ActionsControl
		{
			get { return (ContextActionsHeaderedControl)this.Child; }
			set { this.Child = value; }
		}
		
		public ContextActionsViewModel Actions
		{
			get { return (ContextActionsViewModel)ActionsControl.DataContext; }
			set {
				ActionsControl.DataContext = value;
			}
		}
		
		public new void Focus()
		{
			this.ActionsControl.Focus();
		}
		
		public void OpenAtCaretAndFocus()
		{
			ITextEditor currentEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (currentEditor == null) {
				this.Placement = PlacementMode.MousePoint;
			} else {
				SetPosition(this, currentEditor, currentEditor.Caret.Line, currentEditor.Caret.Column, true);
			}
			this.IsOpen = true;
			this.Focus();
		}
		
		public static void SetPosition(Popup popup, ITextEditor editor, int line, int column, bool openAtWordStart = false)
		{
			var editorUIService = editor == null ? null : editor.GetService(typeof(IEditorUIService)) as IEditorUIService;
			if (editorUIService != null) {
				var document = editor.Document;
				int offset = document.GetOffset(line, column);
				if (openAtWordStart) {
					int wordStart = document.FindPrevWordStart(offset);
					if (wordStart != -1) {
						var wordStartLocation = document.GetLocation(wordStart);
						line = wordStartLocation.Line;
						column = wordStartLocation.Column;
					}
				}
				var caretScreenPos = editorUIService.GetScreenPosition(line, column);
				popup.HorizontalOffset = caretScreenPos.X;
				popup.VerticalOffset = caretScreenPos.Y;
				popup.Placement = PlacementMode.Absolute;
			} else {
				// if no editor information, open at mouse positions
				popup.Placement = PlacementMode.MousePoint;
			}
		}
	}
}