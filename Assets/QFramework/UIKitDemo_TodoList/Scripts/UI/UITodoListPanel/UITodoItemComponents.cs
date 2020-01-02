/****************************************************************************
 * 2018.8 凉鞋的MacBook Pro (2)
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace QFramework.TodoListDemo
{
	public partial class UITodoItem
	{
		[SerializeField] public Toggle IsFinished;
		[SerializeField] public Text Title;

		public void Clear()
		{
			IsFinished = null;
			Title = null;
		}

		public override string ComponentName
		{
			get { return "UITodoItem";}
		}
	}
}
