import React, { Component } from 'react';
import { Navbar, NavbarBrand } from 'reactstrap';
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
        return (
            <header>
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom mb-3" container light>
                    <div className="navbar-content-container">
                        <div className="text-center-container">
                            <NavbarBrand tag={Link} to="/" className="navbar-title">Easy Health Demo</NavbarBrand>
                        </div>
                    </div>
                </Navbar>
            </header>
        );
    }
}
