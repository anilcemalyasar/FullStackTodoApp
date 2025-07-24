import React from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faPenToSquare } from '@fortawesome/free-solid-svg-icons'
import { faTrash } from '@fortawesome/free-solid-svg-icons'

export const Todo = ({task, deleteTodo, editTodo, toggleComplete, onTodoClick}) => {
    return (
    <div className={`Todo${task.completed ? ' completed' : ''}`}>
        <span onClick={onTodoClick} style={{ cursor: "pointer" }}>
            {task.task}
        </span>
        <button onClick={() => toggleComplete(task.id)}>
            {task.completed ? "✅" : "⬜"}
        </button>
        <button onClick={() => editTodo(task.id)}>Edit</button>
        <button onClick={() => deleteTodo(task.id)}>Delete</button>
    </div>
    )
}