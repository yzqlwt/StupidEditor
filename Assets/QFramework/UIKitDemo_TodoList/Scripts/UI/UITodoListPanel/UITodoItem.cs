/****************************************************************************
 * 2018.8 凉鞋的MacBook Pro (2)
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.TodoListDemo
{
	public partial class UITodoItem : UIElement
	{
		private TodoItem mModel;
		
		public void Init(TodoItem model)
		{
			mModel = model;
			UpdateView();
		}

		void UpdateView()
		{
			Title.text = mModel.Title;
			IsFinished.isOn = mModel.Finished;
		}
		
	}
}