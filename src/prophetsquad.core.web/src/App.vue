<template>
  <v-app>
    <v-app-bar app flat dense clipped-left>
      <v-container class="px-xs-0" >
        <v-row no-gutters>
          <v-col>
            <v-app-bar-nav-icon class="hidden-md-and-up" @click="drawer.show = !drawer.show"></v-app-bar-nav-icon>
            <v-avatar tile class="hidden-sm-and-down">
              <router-link to="/" tag="img" :src='require("./assets/logo.png")' :style="{ cursor: 'pointer'}" />
            </v-avatar>
            <v-btn text tile depressed height="100%" small to="fixtures" class="ml-3 hidden-sm-and-down">fixtures</v-btn>
            <v-btn text tile depressed height="100%" small to="leagues" class="hidden-sm-and-down">leagues</v-btn>
          </v-col>
          <v-col cols="10" md="auto">
            <SearchBar placeholder="Teams / Competitions..." />
          </v-col>
          <v-col align="right" class="hidden-sm-and-down">
            <v-btn text tile depressed height="100%" small to="test">log in</v-btn>
            <v-btn text tile depressed outlined color="primary" small to="register">sign up</v-btn>
            <v-btn text tile depressed height="100%" small to="blog">blog</v-btn>
            <v-btn text tile depressed height="100%" small to="about">about</v-btn>
          </v-col>
        </v-row>
      </v-container>
    </v-app-bar>

    <v-navigation-drawer app clipped v-model="drawer.show" class="hidden-md-and-up">

    </v-navigation-drawer>

    <!-- Sizes your content based upon application components -->
    <v-content>
      <!-- Provides the application the proper gutter -->
      <v-container fluid>
        <!-- If using vue-router -->
        <router-view></router-view>
      </v-container>
    </v-content>

    <v-footer app>
      <!-- -->
    </v-footer>
  </v-app>
</template>

<script>

import SearchBar from "@/components/SearchBar.vue";

export default {
  name: "App",
  components: {
    SearchBar
  },

  data: () => ({
    drawer: {
      show: false
    }
  })
};
</script>
