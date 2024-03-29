import { Autocomplete, Button, Card, CardActions, CardContent, IconButton, TextField } from "@mui/material";
import { Container } from "@mui/system";
import { useCallback, useContext, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import axios, { AxiosError, AxiosResponse } from "axios";
import { BACKEND_API_URL } from "../../constants";
import { debounce } from "lodash";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { Painting } from "../../models/Painting";
import { Artist } from "../../models/Artist";
import { SnackbarContext } from "../SnackbarContext";
import { getAuthToken } from "../../auth";

export const PaintingAdd = () => {
	const navigate = useNavigate();
	const openSnackbar = useContext(SnackbarContext);
	const [artists, setArtists] = useState<Artist[]>([]);

	const [painting, setPainting] = useState<Painting>({
		title: "",
        creationYear: 1900,
        height: 0.0,
        subject: "",
        medium: "",
        description: "",
        price: 0.0,
        artistId: 0
	});

	const addPainting = async (event: { preventDefault: () => void }) => {
        event.preventDefault();
        try {
            await axios
                .post(`${BACKEND_API_URL}/paintings`, painting, {
                    headers: {
                        Authorization: `Bearer ${getAuthToken()}`,
                    },
                })
                .then(() => {
                    openSnackbar("success", "Painting added successfully!");
                    navigate("/paintings");
                })
                .catch((reason: AxiosError) => {
                    console.log(reason.message);
                    openSnackbar(
                        "error",
                        "Failed to add painting!\n" +
                            (String(reason.response?.data).length > 255
                                ? reason.message
                                : reason.response?.data)
                    );
                });
        } catch (error) {
            console.log(error);
            openSnackbar(
                "error",
                "Failed to add painting due to an unknown error!"
            );
        }
    };

	useEffect(() => {
        const fetchArtists = async () => {
            try {
                const response = await axios.get<Artist[]>(
                    `${BACKEND_API_URL}/artists/0/10`,
                    {
                        headers: {
                            Authorization: `Bearer ${getAuthToken()}`,
                        },
                    }
                );
                setArtists(response.data);
            } catch (error) {
                console.log("Error fetching artists:", error);
            }
        };
        fetchArtists();
    }, []);

	const fetchSuggestions = async (query: string) => {
        try {
            const response = await axios.get<Artist[]>(
                `${BACKEND_API_URL}/artists/search?query=${query}`,
                {
                    headers: {
                        Authorization: `Bearer ${getAuthToken()}`,
                    },
                }
            );

            const data = response.data;
            const removedDupes = data.filter(
                (v, i, a) => a.findIndex((v2) => v2.name === v.name) === i
            );

            setArtists(removedDupes);
        } catch (error) {
            console.error("Error fetching artist suggestions:", error);
        }
    };

    const debouncedFetchArtistSuggestions = useCallback(debounce(fetchSuggestions, 250), []);

    useEffect(() => {
        return () => {
            debouncedFetchArtistSuggestions.cancel();
        };
    }, [debouncedFetchArtistSuggestions]);

    const handleInputChange = (event: any, value: any, reason: any) => {
		if (value.length < 3) return;
        console.log("input", value, reason);

        if (reason === "input") {
            debouncedFetchArtistSuggestions(value);
        }
    }; 

	return (
		<Container>
			<Card>
				<CardContent sx={{ p: 2 }}>
					<IconButton component={Link} sx={{ mr: 3 }} to={`/paintings`}>
						<ArrowBackIcon />
					</IconButton>{" "}
					<h1>Add Painting</h1>
					<form id="addPaintingForm" onSubmit={addPainting}>
                    	<TextField
							id="title"
							label="Title"
							variant="outlined"
							fullWidth
							sx={{ mb: 2 }}
							onChange={(event) => setPainting({ ...painting, title: event.target.value })}
						/>
						<TextField
							id="creationYear"
							label="Creation Year"
							variant="outlined"
							fullWidth
							sx={{ mb: 2 }}
							onChange={(event) => setPainting({ ...painting, creationYear: parseInt(event.target.value) })}
						/>
                        <TextField
							id="height"
							label="Height"
							variant="outlined"
							fullWidth
							sx={{ mb: 2 }}
							onChange={(event) => setPainting({ ...painting, height: parseFloat(event.target.value) })}
						/>
                        <TextField
							id="subject"
							label="Subject"
							variant="outlined"
							fullWidth
							sx={{ mb: 2 }}
							onChange={(event) => setPainting({ ...painting, subject: event.target.value })}
						/>
                        <TextField
							id="medium"
							label="Medium"
							variant="outlined"
							fullWidth
							sx={{ mb: 2 }}
							onChange={(event) => setPainting({ ...painting, medium: event.target.value })}
						/>
                        <TextField
							id="description"
							label="Description"
							variant="outlined"
							fullWidth
							sx={{ mb: 2 }}
							onChange={(event) => setPainting({ ...painting, description: event.target.value })}
						/>
                        <TextField
							id="price"
							label="Price"
							variant="outlined"
							fullWidth
							sx={{ mb: 2 }}
							onChange={(event) => setPainting({ ...painting, price: parseFloat(event.target.value) })}
						/>
                        <Autocomplete
                            id="artistId"
                            sx={{ mb: 2 }}
                            options={artists}
                            getOptionLabel={(option) => `${option.firstName} ${option.lastName}`}
                            renderOption={(props, option) => {
                                return (
                                    <li {...props} key={option.id}>
                                        {option.name}
                                    </li>
                                );
                            }}
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    label="Artist"
                                    variant="outlined"
                                />
                            )}
                            filterOptions={(x) => x}
                            onInputChange={handleInputChange}
                            onChange={(event, value) => {
                                if (value) {
                                    console.log(value);
                                    setPainting({
                                        ...painting,
                                        artistId: value.id ?? 0,
                                    });
                                }
                            }}
                        />
					</form>
				</CardContent>
				<CardActions sx={{ mb: 1, ml: 1, mt: 1 }}>
					<Button type="submit" variant="contained" id="addPaintingForm">Add Painting</Button>
				</CardActions>
			</Card>
		</Container>
	);
};
