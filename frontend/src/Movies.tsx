import React, { useEffect, useState } from 'react';
import { apiService } from './services/api';
import { Media } from './types';
import MediaOverview from './components/MediaOverview';

const Movies = () => {
    const [movies, setMovies] = useState<Media[]>([]);
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
                const data = await apiService.getMovies(page);
                setMovies(data.items);
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
                <p className="text-2xl">loading movies...</p>
            ) : (
                <div>
                    <h2 className="text-2xl text-center font-medium text-disney-blue mb-4">Movies and Shorts</h2>
                    <MediaOverview medias={movies} />
                    <div className={`mt-8 flex ${hasPrev ? 'justify-between' : 'justify-end'}`}>
                        <button onClick={loadPrev} className={`border border-gray-500 p-2 rounded-lg ${!hasPrev ? 'hidden' : ''}`}>prev page</button>
                        <button onClick={loadNext} className={`border border-gray-500 p-2 rounded-lg ${!hasNext ? 'hidden' : ''}`}>next page</button>
                    </div>
                </div>
            )}
            
        </div>
    );
};

export default Movies;