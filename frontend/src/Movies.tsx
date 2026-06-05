import React, { useEffect, useState } from 'react';
import { apiService } from './services/api';
import { Media } from './types';
import MediaOverview from './components/MediaOverview';
import Pagination from './components/Pagination';

const Movies = () => {
    const [movies, setMovies] = useState<Media[]>([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [hasPrev, setPrev] = useState(false);
    const [hasNext, setNext] = useState(false);
    const loadNext = () => {
        setLoading(true);
        setPage(current => current + 1);
    }
    const loadPrev = () => {
        setLoading(true);
        setPage(current => current - 1);
    }
    const update = (next: boolean) => {
        window.scrollTo({top: 0, behavior: 'smooth'});
        if (next) {
            loadNext();
        } else {
            loadPrev();
        }
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
                    <h2 className="text-2xl text-center font-medium text-disney-blue mb-4">Movies</h2>
                    <MediaOverview medias={movies} />
                    <Pagination hasNext={hasNext} hasPrev={hasPrev} update={update} />
                </div>
            )}
            
        </div>
    );
};

export default Movies;