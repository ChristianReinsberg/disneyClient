import { CharacterDetails, MediaDetails, PagedResult } from "../types";

const API_BASE_URL = 'http://localhost:5000/api';

export const apiService = {
    getCharacters: async (page: number): Promise<PagedResult> => {
        const response = await fetch(`${API_BASE_URL}/character?page=${page}`);
        if (!response.ok) {
            throw new Error('error loading characters');
        }
        return response.json();
    },

    getMedia: async (page: number): Promise<PagedResult> => {
        const response = await fetch(`${API_BASE_URL}/media/all?page=${page}`);
        if (!response.ok) {
            throw new Error('error loading all media');
        }
        return response.json();
    },

    getMovies: async (page: number): Promise<PagedResult> => {
        const response = await fetch(`${API_BASE_URL}/media/movies?page=${page}`);
        if (!response.ok) {
            throw new Error('error loading all movies');
        }
        return response.json();
    },

    getShorts: async (page: number): Promise<PagedResult> => {
        const response = await fetch(`${API_BASE_URL}/media/shorts?page=${page}`);
        if (!response.ok) {
            throw new Error('error loading all shorts');
        }
        return response.json();
    },

    getSeries: async (page: number): Promise<PagedResult> => {
        const response = await fetch(`${API_BASE_URL}/media/series?page=${page}`);
        if (!response.ok) {
            throw new Error('error loading all series');
        }
        return response.json();
    },

    getCharacterDetails: async (id: number): Promise<CharacterDetails> => {
        const response = await fetch(`${API_BASE_URL}/character/${id}`);
        if (!response.ok) {
            throw new Error('error loading character details');
        }
        return response.json();
    },

    getMovieDetails: async (id: number): Promise<MediaDetails> => {
        const response = await fetch(`${API_BASE_URL}/media/movie/${id}`);
        if (!response.ok) {
            throw new Error('error loading movie details');
        }
        return response.json();
    },

    getShortDetails: async (id: number): Promise<MediaDetails> => {
        const response = await fetch(`${API_BASE_URL}/media/short/${id}`);
        if (!response.ok) {
            throw new Error('error loading short details');
        }
        return response.json();
    },

    getSeriesDetails: async (id: number): Promise<MediaDetails> => {
        const response = await fetch(`${API_BASE_URL}/media/series/${id}`);
        if (!response.ok) {
            throw new Error('error loading series details');
        }
        return response.json();
    }
}