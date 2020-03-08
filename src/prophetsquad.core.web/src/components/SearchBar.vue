<template>
  <v-form v-on:submit="submit">
    <v-text-field
      full-width
      single-line
      dense
      clearable
      hide-details
      prepend-inner-icon="mdi-magnify"
      v-model="searchText"
      :placeholder="placeholder"
    ></v-text-field>
  </v-form>
</template>

<script>
export default {
  name: "SearchBar",
  data: () => ({
    searchText: null
  }),
  props: ["placeholder"],
  methods: {
    submit() {
      this.$router.push("/search?" + this.searchText);
    }
  }
};
</script>