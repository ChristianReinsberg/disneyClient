import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import './index.css';
import Characters from './Characters';
import Medias from './Media';
import Movies from './Movies';
import Series from './Series';
import Shorts from './Shorts';
import CharacterDetails from './CharacterDetails';
import MediaDetail from './MediaDetails';
import TmdbLogo from './assets/tmdb_logo.svg';

const NavigationHeader = () => {
  return (
    <header className="my-8 mx-8 flex gap-8 items-end">
      <h1 className="text-3xl font-medium text-disney-blue">Disney Library</h1>
      <nav>
        <ul className="flex gap-4">
          <li><Link to="/" className="text-disney-blue hover:underline text-lg font-medium">Characters</Link></li>
          <li><Link to="/media" className="text-disney-blue hover:underline text-lg font-medium">All Media</Link></li>
          <li><Link to="/movies" className="text-disney-blue hover:underline text-lg font-medium">Movies</Link></li>
          <li><Link to="/shorts" className="text-disney-blue hover:underline text-lg font-medium">Shorts</Link></li>
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
        <Route path="/shorts" element={<Shorts />} />
        <Route path="/movie/:id" element={<MediaDetail />} />
        <Route path="/series/:id" element={<MediaDetail />} />
        <Route path="/shorts/:id" element={<MediaDetail />} />
      </Routes>
      <footer className="bg-white p-4">
        <p className="text-center font-medium">&copy; Christian Reinsberg 2026</p>
        <p className="text-center">Made using data from</p>
        <img src={TmdbLogo} className="w-90 block mx-auto" alt="The Movie Database Logo" />
      </footer>
    </BrowserRouter>
  );
}