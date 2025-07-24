import { useLocation, useParams } from "react-router-dom";

const TodoDetail = () => {
    const { id } = useParams();
    const location = useLocation();
    const todo = location.state?.todo;

    if (!todo) {
        return <div>Todo not found.</div>;
    }

    return (
        <div>
            <h2>Todo Detail</h2>
            {/* <p><strong>ID:</strong> {todo.id}</p> */}
            <p><strong>Title:</strong> {todo.task}</p>
            <p><strong>Description:</strong> {todo.description}</p>
            <p><strong>Completed:</strong> {todo.completed ? "Yes" : "No"}</p>
            <p><strong>Created:</strong> {new Date(todo.createdDate).toLocaleString('tr-TR', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit',
                hour12: false
            })}</p>
        </div>
    );
};

export default TodoDetail;