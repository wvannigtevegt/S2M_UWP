﻿using S2M.Models;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace S2M.Controls {
	public sealed partial class TagCheckinTemplate : UserControl {
		public CheckInKnowledgeTag CheckInKnowledgeTag { get { return this.DataContext as CheckInKnowledgeTag; } }

		public TagCheckinTemplate() {
			this.InitializeComponent();

			this.DataContextChanged += (s, e) => Bindings.Update();
		}
	}
}
