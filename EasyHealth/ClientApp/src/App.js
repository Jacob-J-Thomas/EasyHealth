import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import HomePage from './components/HomePage';
import './App.css';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Routes>
                    <Route path="/" element={<HomePage />} />
                </Routes>
            </Layout>
        );
    }
}
