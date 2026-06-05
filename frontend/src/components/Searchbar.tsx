import React, { useEffect, useState } from 'react';
import { SearchEntry } from '../types';
import SearchIcon from '../assets/search.svg';
import SearchListEntry from './SearchListEntry';
import { apiService } from '../services/api';

const Searchbar = () => {
    const [search, setSearch] = useState('');
    const [characterEntries, setCharacterEntries] = useState<SearchEntry[]>([]);
    const [movieEntries, setMovieEntries] = useState<SearchEntry[]>([]);
    const [seriesEntries, setSeriesEntries] = useState<SearchEntry[]>([]);
    const [shortEntries, setShortEntries] = useState<SearchEntry[]>([]);
    const openModal = () => {
        (document.getElementById('searchModal') as HTMLDialogElement).showModal();
    }

    useEffect(() => {
        const loadEntries = async () => {
            try {
                const data = await apiService.getSearchEntries(search);
                setCharacterEntries(data.suggestions.filter(entry => entry.type === 'Character'));
                setMovieEntries(data.suggestions.filter(entry => entry.type === 'Movie'));
                setSeriesEntries(data.suggestions.filter(entry => entry.type === 'TV'));
                setShortEntries(data.suggestions.filter(entry => entry.type === 'Short'));
            } catch (error) {
                console.error('could not load searchentries', error);
            }
        }
        if (search !== '') {
            loadEntries();
        }
    }, [search]);

    return (
        <div>
            <button onClick={openModal} className="rounded-lg border border-gray-600 px-4 py-2 appearance-none flex gap-2 cursor-pointer"><img className="w-4 aspect-square" src={SearchIcon} alt="search" />Search for Characters or Media</button>
            <dialog id="searchModal" className="fixed top-0 left-0 bg-white mx-auto w-screen h-dvh p-4 pt-8">
                <label className="py-4 block w-full">
                    <span className="font-medium text-disney-blue text-2xl pb-2 block">Search for Character or Media:</span>
                    <input className="rounded-lg px-4 py-2 border border-gray-600 appearance-none w-90" vlaue={search} onChange={e => setSearch(e.target.value)} />
                </label>
                <hr />
                <SearchListEntry entries={characterEntries} title='Characters' />
                <SearchListEntry entries={movieEntries} title='Movies' />
                <SearchListEntry entries={shortEntries} title='Shorts' />
                <SearchListEntry entries={seriesEntries} title='Series' />
            </dialog>
        </div>
    )
}

export default Searchbar;