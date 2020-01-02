/****************************************************************************
 * 2018.8 凉鞋的MacBook Pro (2)
 ****************************************************************************/

namespace QFramework.TodoListDemo
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class UITodoListPanel
	{
		public const string NAME = "UITodoListPanel";

		public UITodoItem UITodoItem;
		public RectTransform Content;
		public Button BtnAddTodoItem;

		protected override void ClearUIComponents()
		{
			UITodoItem = null;
			Content = null;
			BtnAddTodoItem = null;
		}

		private UITodoListPanelData mPrivateData = null;

		public UITodoListPanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new UITodoListPanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
