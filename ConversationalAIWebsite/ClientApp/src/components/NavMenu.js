import React, { Component } from 'react';
import { Navbar, NavbarBrand, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import AuthButton from './AuthButton';
import './NavMenu.css';

export class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor() {
        super();
        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true
        };
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    render() {
        const { pathname } = window.location;

        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom mb-3" container light>
                    <div className="navbar-content-container">
                        <div className="d-flex flex-column align-items-start">
                            <NavbarBrand tag={Link} to="/api">NLSequel</NavbarBrand>
                            <h3 className="navbar-subtitle">A Natural Language to SQL Conversion Tool</h3>
                        </div>
                        <div className="nav-collapse-bottom">
                            <div className="logout-container">
                                <AuthButton />
                            </div>
                            <div className="navbar-nav">
                                <NavItem>
                                    <NavLink tag={Link} className={`text-dark ${pathname === '/' || pathname === '/home' ? 'active' : ''}`} to="/home">Home</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className={`text-dark ${pathname === '/api' ? 'active' : ''}`} to="/api">Convert SQL</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className={`text-dark ${pathname === '/reference' ? 'active' : ''}`} to="/reference">API Reference</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/stripe-portal">Manage Subscription</NavLink>
                                </NavItem>
                            </div>
                        </div>
                    </div>
                </Navbar>
            </header>
        );
    }
}
