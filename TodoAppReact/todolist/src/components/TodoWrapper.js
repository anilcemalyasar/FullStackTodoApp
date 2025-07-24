import React, { useEffect } from 'react'
import TodoForm from './TodoForm'
import { useState } from 'react'
import { v4 as uuidv4 } from 'uuid'
import { Todo } from './Todo';
import { EditTodoForm } from './EditTodoForm';
import axios from 'axios';
import { API_BASE_URL } from '../config/Api'; // Import the API base URL from the config
import { useNavigate } from 'react-router-dom';
uuidv4();

const TodoWrapper = () => {

    const navigate = useNavigate();
    const [todos, setTodos] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    // useEffect(() => {
    //     fetch('http:/localhost:7159/api/todos')
    //     .then(response => response.json())
    //     .then(data => setTodos(data))
    //     .catch(error => console.error('Error fetching todos:', error));
    // }, []);
    
    console.log('API_BASE_URL:', API_BASE_URL);

    useEffect(() => {
        fetchTodos();
    }, []); // Dependency array should be empty to run only once on mount

    const fetchTodos = async () => {
        setLoading(true);
        try {
            const response = await axios.get(`${API_BASE_URL}/todos`);
            
            // map .net todos to react todos
            const mappedTodos = response.data.map(todo => ({
                id: todo.id,
                task: todo.title,
                description: todo.description,
                completed: todo.completed,
                isEditing: false,
                createdDate: todo.createdDate
            }));
            setTodos(mappedTodos)
        } catch (error) {
            setError(error);
            console.error('Error fetching todos:', error);
        } finally {
            setLoading(false);
        }
    }
    
    const addTodo = async (todoText, description) => {
        try {
            const newTodo = {
                title: todoText,
                description: description,
                isCompleted: false
            }
            
            const response = await axios.post(`${API_BASE_URL}/todos`, newTodo);

            // post api hata dönmediyse yeni todoyu listeye ekleyelim
            const insertedTodo = response.data;
            const formattedTodo = {
                id: insertedTodo.id,
                task: insertedTodo.title,
                description: insertedTodo.description,
                completed: insertedTodo.isCompleted,
                isEditing: false,
                createdDate: insertedTodo.createdDate
            };

            setTodos([...todos, formattedTodo]);
        } catch (error) {
            console.error('Error adding todo:', error);
            setError('Todo eklenirken hata oluştu!');
        } finally {
            // Optionally, you can handle loading state here if needed
            setLoading(false);
        }
    }

    // const addTodo = (todo) => {
    //     setTodos([
    //         ...todos,
    //         { id: uuidv4(), task: todo, completed: false, isEditing: false },
    //     ]);
    //     console.log(todos);
    // }

    const toggleComplete = async (id) => {

    try {
        const todo = todos.find(todo => todo.id === id);
        if (!todo) return;

        // Update the completed status of the todo
        const updatedTodo = {
            ...todo,
            title: todo.task,
            description: todo.description,
            isCompleted: !todo.completed
        };

        const response = await axios.put(`${API_BASE_URL}/todos/${id}`, updatedTodo);

        setTodos(
            todos.map(todo => todo.id === id ?
                { ...todo, completed: response.data.isCompleted } 
                : todo
            )
        );
    }
    catch (error) {
        console.error('Error toggling todo completion:', error);
        setError('Todo tamamlanma durumu güncellenirken hata oluştu!');
        }
    }
    // const deleteTodo = (id) => {
    //     setTodos(todos.filter(todo => todo.id !== id))
    // }
    const deleteTodo = async (id) => {
        try {
            await axios.delete(`${API_BASE_URL}/todos/${id}`)
            setTodos(todos.filter(todo => todo.id !== id))
        } catch (error) {
            console.error('Error deleting todo:', error);
            setError('Todo silinirken hata oluştu!');
        }
    }

    // aynı title ve description ile todo eklenmesini engellemek için
    const editTask = async (newTaskText, id) => {
        try {
            const todo = todos.find(todo => todo.id === id);
            if (!todo) return;

            const updatedTodo = {
                ...todo,
                title: newTaskText,
                description: todo.description,
                isCompleted: todo.completed
            };

            const response = await axios.put(`${API_BASE_URL}/todos/${id}`, updatedTodo);
            
            setTodos(todos.map(todo => todo.id === id ? 
                { ...todo, isEditing: false, 
                    task: response.data.title,
                    description:updatedTodo.description,
                    completed: response.data.isCompleted } 
                : todo
            ));
        } catch (error) {
            console.error('Error editing todo:', error);
            setError('Todo düzenlenirken hata oluştu!');
        }
    }

    const editTodo = (id) => {
        setTodos(
            todos.map(todo => todo.id === id ? 
                { ...todo, isEditing: !todo.isEditing } 
                : todo
            )
        );
        // After editing, we set isEditing back to false
    }

    const handleTodoClick = (todo) => {
        navigate(`/todos/${todo.id}`, { state: { todo } });
        // This will navigate to the specific todo's detail page
    }
    
    if (loading && todos.length === 0) {
        return <div className='TodoWrapper'>
            <h1>Loading...</h1>
        </div>
    } 

    return (
    <div className='TodoWrapper'>
        <h1>Get Things Done!</h1>
        <TodoForm addTodo={addTodo} loading={loading}/>
        {todos.map((todo) => {
            return todo.isEditing ? 
                (<EditTodoForm editTodo={editTask} key={todo.id} task={todo}/> )
                :
                (<Todo 
                    task={todo} 
                    key={todo.id} 
                    editTodo={editTodo} 
                    deleteTodo={deleteTodo} 
                    toggleComplete={toggleComplete}
                    onTodoClick={() => handleTodoClick(todo)} />)
            }
        )}
    </div>
    )
}

export default TodoWrapper