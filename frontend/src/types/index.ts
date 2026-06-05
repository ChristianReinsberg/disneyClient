export type PagedResult = {
    items: Character[] | Media[];
    pageNumber: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPrevPage: boolean;
}

export interface Character {
    id: number;
    name: string;
    imageUrl: string;
    films: string[];
    shortFilms: string[];
    tvShows: string[];
    medias: Media[];
}

export type Media = {
    id: number;
    mediaType: MediaType;
    overview: string;
    posterPath: string;
    releaseDate: string;
    name: string;
    voteAvg: number;
    voteCount: number;
    characters: Character[];
}

export type MediaType = 'Movie' | 'TV' | 'Short' | 'Character';

export interface CharacterDetails{
    id: number;
    name: string;
    imageUrl: string;
    medias: ShortMediaDetails[];
}

export interface ShortMediaDetails {
    id: string;
    title: string;
    mediaType: MediaType;
    posterPath: string;
}

export interface MediaDetails {
    id: number;
    mediaType: MediaType;
    title: string;
    overview: string;
    posterPath: string;
    voteAvg: number;
    voteCount: number;
    releaseDate: string;
    characters: ShortCharacterDetails[];
}

export interface ShortCharacterDetails {
    id: number;
    name: string;
    imageUrl: string;
}

export interface SearchEntry {
    id: number;
    name: string;
    type: MediaType;
}

export interface SearchResult {
    suggestions: SearchEntry[];
}