import { useLocation, useParams, useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faLongArrowLeft } from '@fortawesome/free-solid-svg-icons';

const TodoDetail = () => {
    const { id } = useParams();
    const location = useLocation();
    const navigate = useNavigate();
    const todo = location.state?.todo;

    if (!todo) {
        return <div className="todo-detail-notfound">Todo not found.</div>;
    }

    return (
        <div className="TodoDetailWrapper">
            <span
                className="todo-detail-back"
                title="Back to Todos"
                onClick={() => navigate('/todos')}
            >
                <FontAwesomeIcon icon={faLongArrowLeft} /> Back
            </span>
            <h2 className="todo-detail-title">Todo Detail</h2>
            <table className="todo-detail-table">
                <tbody>
                    <tr>
                        <th>Title</th>
                        <td>{todo.task}</td>
                    </tr>
                    <tr>
                        <th>Description</th>
                        <td>{todo.description}</td>
                    </tr>
                    <tr>
                        <th>Completed</th>
                        <td>{todo.completed ? "Yes" : "No"}</td>
                    </tr>
                    <tr>
                        <th>Created</th>
                        <td>
                            {new Date(todo.createdDate).toLocaleString('tr-TR', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric',
                                hour: '2-digit',
                                minute: '2-digit',
                                second: '2-digit',
                                hour12: false
                            })}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    );
};

export default TodoDetail;