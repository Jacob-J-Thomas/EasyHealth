import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import ProtectedWindowWrapper from './components/ProtectedWindowWrapper';
import ApiReference from './components/ApiReference';
import HomePage from './components/HomePage';
import Callback from './components/Callback';
import './App.css';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Routes>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/api" element={<ProtectedWindowWrapper />} />
                    <Route path="/reference" element={<ApiReference />} />
                    <Route path="/home" element={<HomePage />} />
                    <Route path="/callback" element={<Callback />} />
                </Routes>
            </Layout>
        );
    }
}