import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
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
                            <NavbarBrand tag={Link} to="/">NLSequal</NavbarBrand>
                            <h3 className="navbar-subtitle">A Natural Language to SQL Conversion Tool</h3>
                        </div>
                        <div className="nav-collapse-bottom">
                            <div className="navbar-nav">
                                <NavItem>
                                    <NavLink tag={Link} className={`text-dark ${pathname === '/' ? 'active' : ''}`} to="/api">Convert SQL</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className={`text-dark ${pathname === '/api' ? 'active' : ''}`} to="/">API Reference</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className={`text-dark ${pathname === '/api' ? 'active' : ''}`} to="/">Contact Us</NavLink>
                                </NavItem>
                            </div>
                        </div>
                    </div>
                </Navbar>
            </header>
        );
    }
}
