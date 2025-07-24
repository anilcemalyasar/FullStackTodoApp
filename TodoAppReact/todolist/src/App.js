import TodoWrapper from "./components/TodoWrapper";
import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import TodoDetail from "./components/TodoDetail";

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<TodoWrapper />} />
          <Route path="/todos" element={<TodoWrapper />} />
          <Route path="/todos/:id" element={<TodoDetail />} />
        </Routes>
      </BrowserRouter>     
    </div>
  );
}

export default App;
