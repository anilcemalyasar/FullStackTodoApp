import React from 'react'
import { useState } from 'react'
import axios from 'axios';
import { API_BASE_URL } from '../config/Api'; // Import the API base URL from the config

const TodoForm = ({addTodo, loading}) => {

    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [aiLoading, setAiLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (title.trim()) {
            console.log(title);
            await addTodo(title.trim(), description.trim());

            setTitle("");
            setDescription("");
        }
    }

    const getTodoSuggestionByAI = async () => {
        setAiLoading(true);
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get(`${API_BASE_URL}/ai/suggest-todo`, {
                headers: {
                    Authorization: `Bearer ${token}` // Include the token in the request headers
                }
            });
            setTitle(response.data.title); // Assuming the API returns a suggestion in this format
            setDescription(response.data.description); // Assuming the API returns a description
        } catch (error) {
            console.error('Error fetching AI suggestion:', error);
        } finally {
            setAiLoading(false);
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
        <button type='button' className='todo-btn ai-btn' onClick={getTodoSuggestionByAI} disabled={aiLoading || loading}>
            {aiLoading ? "Loading..." : "Get AI Todo Suggestion"}
        </button>
    </form>
    )
}

export default TodoForm