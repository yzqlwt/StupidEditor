/****************************************************************************
 * 2018.8 凉鞋的MacBook Pro (2)
 ****************************************************************************/



namespace QFramework.TodoListDemo
{
	public class UITodoListPanelData : UIPanelData
	{
		public TodoList Model = new TodoList();
	}

	public partial class UITodoListPanel : UIPanel
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as UITodoListPanelData ?? new UITodoListPanelData();
			//please add init code here

			mData.Model.OnDataChanged += OnDataChanged;
			
			OnDataChanged();
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
			BtnAddTodoItem.onClick.AddListener(() =>
			{
				mData.Model.Add(new TodoItem()
				{
					Id = 0,
					Title = "这是添加的 todo",
					Finished = false
				});
			});
		}

		private void OnDataChanged()
		{
			Content.DestroyAllChild();
			
			mData.Model.TodoItems.ForEach(todoItem =>
			{
				UITodoItem
					.Instantiate()
					.Parent(Content)
					.LocalIdentity()
					.ApplySelfTo(selfItem => { selfItem.Init(todoItem); })
					.Show();
			});
		}

		protected override void OnClose()
		{
			mData.Model.OnDataChanged -= OnDataChanged;
		}
	}
}