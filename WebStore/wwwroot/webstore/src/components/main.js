import React, { Component } from 'react';
import './main.css';
import Top from './top'
import Left from './left'
import Center from './center'
import Right from './right'
import Bottom from './bottom'

class Main extends Component {
  render() {
    return (
      <div className="main">
        <div className="top"><Top /></div>
        <div className="content">
          <div className="left"><Left /></div>
          <div className="center"><Center /></div>
          <div className="right"><Right /></div>
        </div>
        <div className="bottom"><Bottom /></div>
      </div>
    );
  }

}

export default Main;
