import React from 'react';
import logo from './logo.svg';
import './App.css';
import Game from './components/game/game';
import Kernel from './core/kernel';

const game = new Kernel();

function App() {
  return (
    <div className="App">
      <Game controller={game.controller} />
      {false && <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.tsx</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
        
      </header>}
    </div>
  );
}

export default App;
