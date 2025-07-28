import React from "react";
import { Navigate, Outlet } from "react-router-dom";

const ProtectedRoute = () => {
    const token = localStorage.getItem("token");
    return ( 
        // <Outlet> component can be used in a parent <Route> element to render out child elements.
        token ? <Outlet /> : <Navigate to="/"/>
    )
};

export default ProtectedRoute;