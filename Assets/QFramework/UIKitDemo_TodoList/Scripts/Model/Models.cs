/****************************************************************************
 * 2018.8 凉鞋的MacBook Pro (2)
 ****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework.TodoListDemo
{
    public class TodoList
    {
        public Action OnDataChanged = () => { };
        
        public List<TodoItem> TodoItems = new List<TodoItem>()
        {
            new TodoItem(){Id = 0,Title = "假的 0",Finished = false},
            new TodoItem(){Id = 1,Title = "假的 1",Finished = true},
            new TodoItem(){Id = 2,Title = "假的 2",Finished = false},
        };

        public void Add(TodoItem todoItem)
        {
            TodoItems.Add(todoItem);
            OnDataChanged();
        }

        public void Remove(TodoItem todoItem)
        {
            TodoItems.Remove(todoItem);
            OnDataChanged();
        }
        
        public void RemoveById(int id)
        {
            TodoItems.RemoveAll(todoItem=>todoItem.Id == id);
            OnDataChanged();
        }
    }

    public class TodoItem
    {
        public int Id;

        public string Title;

        public bool Finished;
    }
}