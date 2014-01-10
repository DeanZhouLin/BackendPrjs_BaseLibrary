using System;
using System.Web.UI.WebControls;

namespace Com.BaseLibrary.Web
{
	public class CommonListBox : CompositeControl
	{
		private RadioButtonList rbList = new RadioButtonList();
		private ListBox listBox = new ListBox();
		private DropDownList ddList = new DropDownList();

		private ListControl m_ListControl;

		protected override void CreateChildControls()
		{
			this.Controls.Clear();

			//rbList = new RadioButtonList();
			//listBox = new ListBox();
			//ddList = new DropDownList();

			rbList.Visible = false;
			listBox.Visible = false;
			ddList.Visible = false;

			this.Controls.Add(this.rbList);
			this.Controls.Add(this.listBox);
			this.Controls.Add(this.ddList);

			ChildControlsCreated = true;
			base.CreateChildControls();
		}

		private RepeatDirection? m_RepeatDirection;
		public virtual RepeatDirection RepeatDirection
		{
			get
			{
				if (m_RepeatDirection == null)
				{
					object obj = ViewState["REPEATDIRECTION"];
					m_RepeatDirection = obj == null ? RepeatDirection.Horizontal : (RepeatDirection)obj;
				}
				return m_RepeatDirection.Value;
			}
			set
			{
				ViewState["REPEATDIRECTION"] = value;
				m_RepeatDirection = value;
			}
		}

		private ListType? m_ListType;
		public ListType ListType
		{
			get
			{
				if (m_ListType == null)
				{
					object obj = ViewState["LISTTYPE"];
					m_ListType = obj == null ? ListType.DropDownList : (ListType)obj;
				}
				return m_ListType.Value;
			}
			set
			{
				ViewState["LISTTYPE"] = value;
				m_ListType = value;
			}
		}

		public virtual ListItemCollection Items { get { return CurrentListControl.Items; } }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			CurrentListControl.Visible = true;
		}

		protected override void DataBind(bool raiseOnDataBinding)
		{
			this.CurrentListControl.DataSource = this.DataSource;
			this.CurrentListControl.DataBind();
		}
		public ListControl CurrentListControl
		{
			get
			{
				if (m_ListControl == null)
				{
					switch (ListType)
					{
						case ListType.DropDownList:
							m_ListControl = ddList;
							break;
						case ListType.RadioButtonList:
							m_ListControl = rbList;
							break;
						case ListType.ListBox:
							m_ListControl = listBox;
							break;
						default:
							m_ListControl = ddList;
							break;
					}
				}
				return m_ListControl;
			}
		}

		private void CreateRadioButtonList()
		{
			RadioButtonList rblist = new RadioButtonList();
			rblist.RepeatDirection = RepeatDirection;
			m_ListControl = rblist;
		}

		private void CreateControlHierarchy()
		{
			this.Controls.Add(CurrentListControl);
		}

		public string SelectedValue
		{
			get
			{
				return CurrentListControl.SelectedValue;
			}
			set
			{
				CurrentListControl.SelectedValue = value;
			}
		}

		public int SelectedIndex
		{
			get
			{
				return CurrentListControl.SelectedIndex;
			}
			set
			{
				CurrentListControl.SelectedIndex = value;
			}
		}



		protected void OnSelectedIndexChanged(EventArgs e)
		{

		}

		public ListItem SelectedItem
		{
			get
			{
				return this.CurrentListControl.SelectedItem;
			}
		}

		public object DataSource
		{
			get
			{
				return this.CurrentListControl.DataSource;
			}
			set
			{
				this.CurrentListControl.DataSource = value;
			}
		}
		protected override void RecreateChildControls()
		{
			base.ChildControlsCreated = false;
			this.Controls.Clear();
			this.Controls.Add(this.CurrentListControl);
			this.ChildControlsCreated = true;
			EnsureChildControls();
		}


		public override void DataBind()
		{
			this.CurrentListControl.DataBind();
		}

		public string DataTextField
		{
			get
			{
				return this.CurrentListControl.DataTextField;
			}
			set
			{
				this.CurrentListControl.DataTextField = value;
			}
		}

		public string DataValueField
		{
			get
			{
				return this.CurrentListControl.DataValueField;
			}
			set
			{
				this.CurrentListControl.DataValueField = value;
			}
		}



	}
}