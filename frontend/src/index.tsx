import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import './index.css';
import Characters from './Characters';
import Medias from './Media';
import Movies from './Movies';
import Series from './Series';
import CharacterDetails from './CharacterDetails';
import MediaDetail from './MediaDetails';

const NavigationHeader = () => {
  return (
    <header className="my-8 mx-8 flex gap-8 items-end">
      <h1 className="text-3xl font-medium text-disney-blue">Disney Library</h1>
      <nav>
        <ul className="flex gap-4">
          <li><Link to="/" className="text-disney-blue hover:underline text-lg font-medium">Characters</Link></li>
          <li><Link to="/media" className="text-disney-blue hover:underline text-lg font-medium">All Media</Link></li>
          <li><Link to="/movies" className="text-disney-blue hover:underline text-lg font-medium">Movies and Shorts</Link></li>
          <li><Link to="/series" className="text-disney-blue hover:underline text-lg font-medium">TV Shows</Link></li>
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
        <Route path="/character/:id" element={<CharacterDetails />} />
        <Route path="/media" element={<Medias />} />
        <Route path="/movies" element={<Movies />} />
        <Route path="/series" element={<Series />} />
        <Route path="/movie/:id" element={<MediaDetail />} />
        <Route path="/series/:id" element={<MediaDetail />} />
      </Routes>
    </BrowserRouter>
  );
}