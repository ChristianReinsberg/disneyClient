import React, { useEffect, useState } from 'react';
import { apiService } from './services/api';
import { useNavigate } from 'react-router-dom';
import { Character } from './types';
import ExtendableList from './components/ExtendableList';

const Characters = () => {
    const navigate = useNavigate();
    const [characters, setCharacters] = useState<Character[]>([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [hasPrev, setPrev] = useState(false);
    const [hasNext, setNext] = useState(false);
    const loadNext = (event: Event) => {
        event.stopPropagation();
        setLoading(true);
        setPage(current => current + 1);
    }
    const loadPrev = (event: Event) => {
        event.stopPropagation();
        setLoading(true);
        setPage(current => current - 1);
    }

    useEffect(() => {
        const loadData = async () => {
            try {
                const data = await apiService.getCharacters(page);
                setCharacters(data.items);
                setPrev(data.hasPrevPage);
                setNext(data.hasNextPage);

            } catch (error) {
                console.error('could not load data', error);
            } finally {
                setLoading(false);
            }
        }
        loadData();
    }, [page]);

    return (
        <div className="min-h-screen bg-gray-50 p-8">

            {loading ? (
                <p className="text-2xl">loading characters...</p>
            ) : (
                <div>
                    <h2 className="text-2xl text-center font-medium text-disney-blue mb-4">Characters</h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 2xl:grid-cols-4 gap-8 w-fit mx-auto">
                        {characters.map((character: Character) => {
                            return (
                                <div title={`Go to detail page of ${character.name}`} onClick={() => navigate(`/character/${character.id}`)} key={character.id} className="col-span-1 w-90 rounded-lg shadow-lg p-4 border bg-white border-gray-200 cursor-pointer">
                                    <img className="aspect-square w-82 object-contain" src={character.imageUrl !== '' ? character.imageUrl : 'https://static.wikia.nocookie.net/disney/images/7/7c/Noimage.png'} alt={character.name} />
                                    <p className="font-medium pt-3 text-disney-blue">{character.name}</p>
                                    {character.films.length > 0 ? <ExtendableList listContent={character.films} name="Movies" />: null}
                                    {character.shortFilms.length > 0 ? <ExtendableList listContent={character.shortFilms} name="Shorts" />: null}
                                    {character.tvShows.length > 0 ? <ExtendableList listContent={character.tvShows} name="TV Shows" />: null}
                                </div>
                            )
                        })}
                    </div>
                    <div className={`mt-8 flex ${hasPrev ? 'justify-between' : 'justify-end'}`}>
                        <button onClick={loadPrev} className={`border border-gray-500 p-2 rounded-lg ${!hasPrev ? 'hidden' : ''}`}>prev page</button>
                        <button onClick={loadNext} className={`border border-gray-500 p-2 rounded-lg ${!hasNext ? 'hidden' : ''}`}>next page</button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Characters;