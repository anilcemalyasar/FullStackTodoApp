import React from 'react'
import { useState } from 'react'

const TodoForm = ({addTodo, loading}) => {

    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (title.trim()) {
            console.log(title);
            await addTodo(title.trim(), description.trim());
        }
    }

    return (
    <form className='TodoForm' onSubmit={handleSubmit}>
        <input className='todo-input' 
            type='text' 
            placeholder='Add a new todo'
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            disabled={loading}
            />
        <input className='todo-input' 
            type='text' 
            placeholder='Add description'
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            disabled={loading}/>    
        <button type='submit' className='todo-btn'>
            {loading ? "Adding..." : "Add Todo"}
        </button>
    </form>
    )
}

export default TodoForm