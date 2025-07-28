import React from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faPenToSquare, faCheckSquare, faSquare, faTrash } from '@fortawesome/free-solid-svg-icons'

export const Todo = ({task, deleteTodo, editTodo, toggleComplete, onTodoClick}) => {
    return (
    <div className={`Todo${task.completed ? ' completed' : ''}`}>
        <span onClick={onTodoClick} style={{ cursor: "pointer" }}>
            {task.task}
        </span>
        <div className="todo-icons">
            <span
                className="icon-btn"
                title="Complete"
                onClick={() => toggleComplete(task.id)}
            >
                <FontAwesomeIcon icon={task.completed ? faSquare : faCheckSquare} />
            </span>
            <span
                className="icon-btn"
                title="Edit"
                onClick={() => editTodo(task.id)}
            >
                <FontAwesomeIcon icon={faPenToSquare} />
            </span>
            <span
                className="icon-btn"
                title="Delete"
                onClick={() => deleteTodo(task.id)}
            >
                <FontAwesomeIcon icon={faTrash} />
            </span>
        </div>
    </div>
    )
}