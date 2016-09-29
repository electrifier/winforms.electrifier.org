using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using WeifenLuo.WinFormsUI.Docking;

using electrifier.Core.Shell32.Services;
using electrifier.Core.Shell32.Controls;
using electrifier.Win32API;

namespace electrifier.Core.Forms.DockControls {
	public delegate void BrowsingAddressChangedEventHandler(object source, EventArgs e);

	/// <summary>
	/// Summary of ShellBrowserDockContent.
	/// </summary>
	public class ShellBrowserDockContent : WeifenLuo.WinFormsUI.Docking.DockContent, IPersistent {
		protected Guid Guid;

		protected ShellTreeView shellTreeView = null;
		protected ShellBrowser shellBrowser = null;
		protected Splitter splitter = null;
		protected string browsingAddress = null;
		public string BrowsingAddress {
			get { return this.browsingAddress; }
		}

		public event BrowsingAddressChangedEventHandler BrowsingAddressChanged = null;
		protected void OnBrowsingAddressChanged() {
			if(this.BrowsingAddressChanged != null)
				this.BrowsingAddressChanged(this, EventArgs.Empty);
		}


		public ShellBrowserDockContent() : this(Guid.NewGuid()) { }

		public ShellBrowserDockContent(Guid guid) : base() {
			// Initialize the underlying DockControl
			this.Guid = guid;
			this.Name = "ShellBrowserDockContent." + Guid.ToString();

			// Initialize ShellTreeView
			this.shellTreeView = new ShellTreeView(ShellAPI.CSIDL.DESKTOP);
			this.shellTreeView.Dock = DockStyle.Left;
			this.shellTreeView.Size = new Size(256, this.Size.Height);
			this.shellTreeView.AfterSelect += new TreeViewEventHandler(shellTreeView_AfterSelect);

			// Initialize ShellBrowser
			this.shellBrowser = new ShellBrowser(shellTreeView.SelectedNode.AbsolutePIDL);
			this.shellBrowser.Dock = DockStyle.Fill;
			this.shellBrowser.BrowseShellObject += new BrowseShellObjectEventHandler(shellBrowser_BrowseShellObject);

			this.UpdateDockCaption();

			// Initialize Splitter
			this.splitter = new Splitter();
			this.splitter.Dock = DockStyle.Left;
			this.splitter.Size = new Size(6, this.Height);

			// Add the controls from right to left
			this.Controls.AddRange(new Control[] { shellBrowser, splitter, shellTreeView });

			this.FormClosed += ShellBrowserDockControl_FormClosed;
		}

		void ShellBrowserDockControl_FormClosed(object sender, FormClosedEventArgs e) {
//			this.DetachFromDockControlContainer();
		}

		// TODO: Dispose when closed!!!

		private void shellTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
			// TODO: TreeViewEventArgs.Node => shellTreeViewNode
			// TODO: ShellTreeView.SelectedNode => shellTreeViewNode
			//this.Cursor = Cursors.WaitCursor;
			this.shellBrowser.NavigateTo(shellTreeView.SelectedNode.AbsolutePIDL);
//			shellListView.SetBrowsingFolder(sender, (shellTreeView.SelectedNode as ShellTreeViewNode).AbsolutePIDL);
			this.UpdateDockCaption();
			this.browsingAddress = this.Text;
			this.OnBrowsingAddressChanged();
			//this.Cursor = Cursors.Default;
		}

		protected void UpdateDockCaption() {
			if (null != this.shellTreeView.SelectedNode) {
				if (this.IsHandleCreated) {
					/*
					 * See http://stackoverflow.com/questions/1167771/methodinvoker-vs-action-for-control-begininvoke
					 * why BeginInvoke((MethodInvoker)...) should be faster than this.BeginInvoke((Action)...)
					 */

					this.BeginInvoke((MethodInvoker)(() => {    // TODO: InvokeRequired
						this.Text = this.shellTreeView.SelectedNode.Text;
						this.Icon = IconManager.GetIconFromPIDL(this.shellTreeView.SelectedNode.AbsolutePIDL, false);
					}));
				} else {
					this.Text = this.shellTreeView.SelectedNode.Text;
					this.Icon = IconManager.GetIconFromPIDL(this.shellTreeView.SelectedNode.AbsolutePIDL, false);
				}
			} else {
				// TODO: Error-Handling
				MessageBox.Show("ShellBrowserDockContent:UpdateDockCaption\n\n'this.shellTreeView.SelectedNode is null'");
			}
		}

		#region IPersistent Member

		public void CreatePersistenceInfo(XmlWriter xmlWriter) {
			xmlWriter.WriteStartElement(this.GetType().Name);
			xmlWriter.WriteStartElement(@"BrowsingAddress");
			xmlWriter.WriteValue(this.BrowsingAddress);
			xmlWriter.WriteEndElement(); // BrowsingAddress
			xmlWriter.WriteEndElement(); // this.GetType().Name
		}

		public void ApplyPersistenceInfo(XmlTextReader xmlReader) {

		}

		#endregion

		private void shellBrowser_BrowseShellObject(object source, BrowseShellObjectEventArgs e) {
			this.shellTreeView.BeginUpdate();

			try {
				// Currently, the IShellView seems to know that a new folder was browsed to,
				// and updates itself accordingly
				//			this.shellBrowser.SetBrowsingFolder(e.ShellObjectPIDL);
				// TODO: The following statements should be ran in another thread
				this.shellTreeView.SelectedNode = this.shellTreeView.FindNodeByPIDL(e.ShellObjectPIDL);


				// TODO: Hehe, i like this feature; however, put something into our configuration dialog
				// to turn this off; also, do this when navigating with the help of the TreeView itself
				if (null != this.shellTreeView.SelectedNode)
					this.shellTreeView.SelectedNode.Expand();
				//this.shellTreeView.SelectedNode.EnsureVisible();
			} finally {
				this.shellTreeView.EndUpdate();
			}
		}
	}
}
