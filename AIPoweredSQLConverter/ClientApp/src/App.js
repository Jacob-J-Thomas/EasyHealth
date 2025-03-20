import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import ProtectedWindowWrapper from './components/ProtectedWindowWrapper';
import ApiReference from './components/ApiReference';
import HomePage from './components/HomePage';
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
                </Routes>
            </Layout>
        );
    }
}