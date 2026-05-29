import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import './index.css';
import Characters from './Characters';

const NavigationHeader = () => {
  return (
    <header className="my-8 mx-8 flex gap-8 items-end">
      <h1 className="text-3xl font-medium text-disney-blue">Disney Library</h1>
      <nav>
        <ul className="flex gap-4">
          <li><Link to="/" className="text-disney-blue hover:underline text-lg font-medium">Characters</Link></li>
        </ul>
      </nav>
  </header>
  )
}

const container = document.getElementById('root');
if (container) {
  const root = createRoot(container);
  root.render(
    <BrowserRouter>
      <NavigationHeader />
      <Routes>
        <Route path="/" element={<Characters />} />
      </Routes>
    </BrowserRouter>
  );
}